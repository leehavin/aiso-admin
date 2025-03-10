namespace AiSo.Admin.Service;

/// <summary>
/// 数据字典服务
/// </summary>
public class SysDictionaryService(IServiceProvider serviceProvider)
    : ApplicationService<SysDictionary, int, SysDictionary, SysDictionary>(serviceProvider)
{
    /// <summary>
    /// 获取列表数据
    /// </summary>
    /// <param name="search"></param>
    /// <returns></returns>
    public async Task<List<SysDictionaryTreeDto>> FindListAsync(SysDictionary search)
    {
        var query = (from sysDictionary in Repository.Select
                    from sysDictionaryParent in Repository.Select.Where(w => w.Id == sysDictionary.ParentId)
                        .DefaultIfEmpty()
                    select new
                    {
                        t1 = sysDictionary,
                        t2 = sysDictionaryParent
                    })
                //.WhereIf(search?.ParentId == 0 || search?.ParentId == null, w => w.t1.ParentId == null || w.t1.ParentId == 0)
                //.WhereIf(search?.ParentId != 0 && search?.ParentId != null, w => w.t1.ParentId == search.ParentId)
                .WhereIf(!string.IsNullOrWhiteSpace(search?.Name), a => a.t1.Name.Contains(search.Name))
                .OrderBy(w => w.t1.Sort)
                .Select(w => new SysDictionaryTreeDto
                {
                    Sort = w.t1.Sort,
                    Code = w.t1.Code,
                    Name = w.t1.Name,
                    Value = w.t1.Value,
                    ParentId = w.t1.ParentId,
                    ParentName = w.t2.Name,
                    LastModificationTime = w.t1.LastModificationTime,
                    CreationTime = w.t1.CreationTime,
                    Id = w.t1.Id,
                })
            ;

        return await query.ToListAsync();
    }

    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    public override async Task SaveFormAsync(SysDictionary form)
    {
        if (await Repository.AnyAsync(w => w.Code == form.Code && w.Id != form.Id))
        {
            throw MessageBox.Show("编码已存在，请勿重复插入");
        }

        await Repository.InsertOrUpdateAsync(form);
    }

    /// <summary>
    /// 获取字典树
    /// </summary>
    /// <returns></returns>
    public async Task<List<SysDictionaryTreeDto>> GetDictionaryTreeAsync()
    {
        var allDictionary = await Repository.ToListAllAsync();
        return CreateDictionaryTree(0, allDictionary);
    }

    /// <summary>
    /// 创建字典树
    /// </summary>
    /// <returns></returns>
    private List<SysDictionaryTreeDto> CreateDictionaryTree(int id, List<SysDictionary> allDictionary)
    {
        List<SysDictionaryTreeDto> result = new();

        var data = new List<SysDictionary>();
        if (id == 0)
        {
            data = allDictionary
                    .Where(w => w.ParentId == null || w.ParentId == 0)
                    .ToList()
                ;
        }
        else
        {
            data = allDictionary
                    .Where(w => w.ParentId == id)
                    .ToList()
                ;
        }

        foreach (var item in data)
        {
            var model = item.MapTo<SysDictionary, SysDictionaryTreeDto>();
            model.Children = CreateDictionaryTree(item.Id, allDictionary);
            result.Add(model);
        }

        return result;
    }

    /// <summary>
    /// 所有字典集合 1小时缓存
    /// </summary>
    /// <returns></returns>
    [Cacheable(CacheDuration = 60 * 60)]
    public Task<List<SysDictionaryDto>> GetDictionaryAllAsync()
    {
        return Repository.SelectNoTracking
            .Select(w => new SysDictionaryDto
            {
                Key = w.Id,
                Code = w.Code,
                Name = w.Name,
                Value = w.Value,
                ParentId = w.ParentId
            })
            .ToListAsync();
    }

    /// <summary>
    /// 根据编码获取下级字典
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<List<SysDictionaryDto>> GetDictionaryByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw MessageBox.Show("参数Code是空!");
        }

        var data = await GetDictionaryAllAsync();

        var parentData = data.FirstOrDefault(w => w.Code == code) ?? throw MessageBox.Show($"编码“{code}”对应找不到字典数据!");
        return data.Where(w => w.ParentId == parentData.Key).ToList();
    }

    /// <summary>
    /// 根据 Code 获取 字典集
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public async Task<List<SysDictionaryTreeDto>> GetDictionaryTreeByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw MessageBox.Show("参数Code是空!");
        }

        var dictionary = await Repository.FindAsync(w => w.Code == code);
        if (dictionary == null) return default;
        var dictionarys = await Repository.Select.Where(w => w.ParentId == dictionary.Id).ToListAsync();
        if (!dictionarys.Any()) return default;
        var result = new List<SysDictionaryTreeDto>();
        return CreateDictionaryTree(dictionary.Id, dictionarys);
    }
}