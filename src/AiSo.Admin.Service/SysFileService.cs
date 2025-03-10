namespace AiSo.Admin.Service;

public class SysFileService : ApplicationService<SysFile, Guid, SysFileSearchDto, SysFile>
{
    private readonly IAccountService _accountService;
    private readonly IFileManager _fileManager;

    public SysFileService(IServiceProvider serviceProvider,
        IAccountService accountService,
        IFileManager fileManager) : base(serviceProvider)
    {
        _accountService = accountService;
        _fileManager = fileManager;
    }

    public override async Task<PagingView> FindListAsync(PagingSearchInput<SysFileSearchDto> pagingSearchInput)
    {
        var query = Repository.SelectNoTracking
            .WhereIf(!string.IsNullOrWhiteSpace(pagingSearchInput.Search.FileName), w => w.OriginalName.Contains(pagingSearchInput.Search.FileName!))
            .WhereIf(pagingSearchInput.Search.FileTypes.Count > 0, w => pagingSearchInput.Search.FileTypes.Contains(w.FileType))
            .OrderByDescending(w => w.CreationTime)
            .Select(w => new SysFileResultDto()
            {
                Id = w.Id,
                Url = w.Url,
                OriginalName = w.OriginalName,
                FileSize = w.FileSize,
                FileType = w.FileType,
                MimeType = w.MimeType,
                Sha = w.Sha,
                CreationTime = w.CreationTime,
                LastModificationTime = w.LastModificationTime
            });

        var result = await Repository.AsPagingViewAsync(query, pagingSearchInput);

        foreach (var item in result.DataSource)
        {
            if (!item.TryGetValue(nameof(SysFileResultDto.Url), out var url))
            {
                continue;
            }
            item[nameof(SysFileResultDto.FullUrl)] = _fileManager.GetServerUrl() + url;
        }

        return result;
    }

    public override async Task<List<Guid>> DeleteListAsync(List<Guid> ids)
    {
        var sysFiles = await Repository.SelectNoTracking.Where(w => ids.Contains(w.Id)).ToListAsync();

        // 从数据库查询不是这些id，但sha相同的文件
        var duplicateFiles = await Repository.SelectNoTracking
            .Where(w => !ids.Contains(w.Id))
            .Where(w => sysFiles.Select(s => s.Sha).Contains(w.Sha))
            .Select(w => new
            {
                w.Id,
                w.Sha,
                w.Url
            })
            .ToListAsync();

        // 查找不是引用同一个文件的，文件地址
        var fileNames = new List<string?>();
        foreach (var item in sysFiles)
        {
            var fileInfo = duplicateFiles.Find(w => w.Sha == item.Sha && w.Url?.ToLower() != item.Url?.ToLower());
            if(fileInfo != null)
            {
                fileNames.Add($"{item.RelativePath}{item.FileName}");
            }
        }

        // 删除文件
        var (deleteFiles, ex) = _fileManager
            .BuildFileManagerContext()
            .FileDelete(fileNames);

        // 如果删除没有报错，删除数据库记录
        if (ex == null)
        {
            await Repository.SelectNoTracking.Where(w => ids.Contains(w.Id)).ExecuteDeleteAsync();
            return ids;
        }

        // 如果删除文件失败，只把删除成功的文件从数据库删除
        var fileIds = sysFiles.Where(w => string.IsNullOrWhiteSpace(w.FileName) || deleteFiles.Contains(w.FileName)).Select(s => s.Id).ToList();

        await Repository.SelectNoTracking.Where(w => fileIds.Contains(w.Id)).ExecuteDeleteAsync();

        return fileIds;
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="uploadFileSaveDto"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <exception cref="MessageBox"></exception>
    public async Task<SysFileResultDto> UploadFileAsync(UploadFileSaveDto uploadFileSaveDto, IFormFile file)
    {
        var sysFileResultDto = new SysFileResultDto();

        var fileManager = _fileManager;
        if (uploadFileSaveDto.Extensions.Count > 0)
        {
            if (uploadFileSaveDto.Extensions.Contains("*"))
            {
                fileManager = fileManager.AllowAnyFileExtensions();
            }
            else
            {
                fileManager = fileManager.ResetFileExtensions().AddFileExtensions(uploadFileSaveDto.Extensions.ToArray());
            }
        }

        if (!string.IsNullOrWhiteSpace(uploadFileSaveDto.RelativePath))
        {
            fileManager = fileManager.AddSaveFolder(uploadFileSaveDto.RelativePath);
        }

        if (uploadFileSaveDto.FileSizeByte != null)
        {
            fileManager = fileManager.AddMaxLength(uploadFileSaveDto.FileSizeByte.Value);
        }

        await fileManager
            .BuildFileManagerContext()
            .FileUploadAsync(file)
            .SuccessAsync(async info =>
            {
                var sysFile = await Repository.InsertAsync(new SysFile()
                {
                    Url = info.FileWebUrl,
                    RelativePath = AddEndWith(uploadFileSaveDto.RelativePath),
                    FileName = info.FileName,
                    OriginalName = info.OldName,
                    FileSize = info.FileSize,
                    FileType = info.FileType,
                    Sha = info.Sha,
                    MimeType = Tools.GetMimeType(info.OldName)
                });

                sysFileResultDto = sysFile.MapTo<SysFile, SysFileResultDto>();
                sysFileResultDto.FullUrl = _fileManager.GetServerUrl() + info.FileWebUrl;
            })
            .ExistsAsync(async info =>
            {
                var fileEntity = await Repository.SelectNoTracking
                    .Where(w => w.Url == info.FileWebUrl && w.OriginalName == info.OldName)
                    .FirstOrDefaultAsync();

                if (fileEntity == null || fileEntity.CreatorUserId != _accountService.AccountContext.Id)
                {
                    fileEntity = await Repository.InsertAsync(new SysFile()
                    {
                        Url = info.FileWebUrl,
                        RelativePath = AddEndWith(uploadFileSaveDto.RelativePath),
                        FileName = info.FileName,
                        OriginalName = info.OldName,
                        FileSize = info.FileSize,
                        FileType = info.FileType,
                        Sha = info.Sha,
                        MimeType = Tools.GetMimeType(info.OldName)
                    });
                }

                sysFileResultDto = fileEntity.MapTo<SysFile, SysFileResultDto>();
                sysFileResultDto.FullUrl = _fileManager.GetServerUrl() + info.FileWebUrl;
            })
            .Error(info => throw MessageBox.CreateMessage(R.ErrorMessage(info.Exception?.Message ?? "上传失败")));

        return sysFileResultDto;
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="MessageBox"></exception>
    public async Task<IActionResult> DownloadFileAsync(Guid id)
    {
        var file = await Repository.SelectNoTracking.FirstOrDefaultAsync(w => w.Id == id);

        if (file == null || string.IsNullOrWhiteSpace(file.FileName))
        {
            throw MessageBox.Show("文件不存在");
        }

        var filePath = _fileManager.BuildFileManagerContext().GetFileRoot(file.FileName);

        if (!File.Exists(filePath))
        {
            throw MessageBox.Show("文件已被删除!");
        }

        await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var memoryStream = new MemoryStream();
        await fileStream.CopyToAsync(memoryStream);

        return new FileContentResult(memoryStream.ToArray(), Tools.GetMimeType(file.MimeType))
        {
            FileDownloadName = WebUtility.UrlEncode(file.OriginalName)
        };
    }

    private static string? AddEndWith(string? str)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return str;
        }

        if (!str.EndsWith('/') || str.EndsWith('\\'))
        {
            return str + "/";
        }

        return str;
    }
}