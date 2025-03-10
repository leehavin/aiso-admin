namespace AiSo.Admin.Service;

/// <summary>
///  服务 SysLocaleService
/// </summary>
public class SysLocaleService(IServiceProvider serviceProvider) : ApplicationService<SysLocale, Guid, SysLocale, SysLocale>(serviceProvider)
{
    /// <summary>
    /// 获取列表数据
    /// </summary>
    /// <param name="pagingSearchInput"></param>
    /// <returns></returns>
    public override async Task<PagingView> FindListAsync(PagingSearchInput<SysLocale> pagingSearchInput)
    {
        var query = this.Repository.SelectNoTracking
                .OrderByDescending(w => w.CreationTime)
                .Select(w => new SysLocale()
                {
                    Id = w.Id,
                    Key = w.Key,
                    Value = w.Value,
                    LastModificationTime = w.LastModificationTime,
                    CreationTime = w.CreationTime
                })
            ;

        var result = await Repository.AsPagingViewAsync(query, pagingSearchInput);

        return result;
    }

    /// <summary>
    /// 获取国际化内容
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Task<string?> FindLocaleAsync(string key)
    {
        return this.Repository.SelectNoTracking
            .Where(w => w.Key == key)
            .Select(w => w.Value)
            .FirstOrDefaultAsync();
    }
}