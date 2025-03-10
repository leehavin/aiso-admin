namespace AiSo.Admin.Service;

/// <summary>
/// 服务 SysDataAuthorityCustomService
/// </summary>
public class SysDataAuthorityCustomService(IServiceProvider serviceProvider) : ApplicationService<SysDataAuthorityCustom, Guid, SysDataAuthorityCustom, SysDataAuthorityCustom>(
    serviceProvider)
{
    /// <summary>
    /// 获取列表数据
    /// </summary>
    /// <param name="pagingSearchInput">page</param>
    /// <returns></returns>
    public override async Task<PagingView> FindListAsync(PagingSearchInput<SysDataAuthorityCustom> pagingSearchInput)
    {
        var query = Repository.Select
                .OrderByDescending(w => w.CreationTime)
                .Select(w => new
                {
                    w.SysDataAuthorityId,
                    w.SysOrganizationId,
                    w.LastModificationTime,
                    w.CreationTime,
                    w.Id,
                })
            ;

        var result = await Repository.AsPagingViewAsync(query, pagingSearchInput);
        //覆盖值
        result
            .FormatValue(query, w => w.CreationTime, (oldValue) => oldValue.ToString("yyyy-MM-dd"))
            .FormatValue(query, w => w.LastModificationTime, (oldValue) => oldValue?.ToString("yyyy-MM-dd"))
            ;

        return result;
    }
}