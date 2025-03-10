namespace AiSo.Admin.Service;

/// <summary>
/// 服务 SysDataAuthorityService
/// </summary>
public class SysDataAuthorityService(
    IServiceProvider serviceProvider,
    IRepository<SysDataAuthorityCustom> sysDataAuthorityCustomRepository)
    : ApplicationService<SysDataAuthority, Guid, SysDataAuthority, SysDataAuthorityFormDto>(serviceProvider)
{
    /// <summary>
    /// 获取列表数据
    /// </summary>
    /// <param name="pagingSearchInput">page</param>
    /// <returns></returns>
    public override async Task<PagingView> FindListAsync(PagingSearchInput<SysDataAuthority> pagingSearchInput)
    {
        var query = Repository.Select
                .OrderByDescending(w => w.CreationTime)
                .Select(w => new
                {
                    w.PermissionType,
                    w.RoleId,
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

    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="form">form</param>
    /// <returns></returns>
    public override async Task SaveFormAsync(SysDataAuthorityFormDto form)
    {
        var sysDataAuthority = await Repository.InsertOrUpdateAsync(form.SysDataAuthority);

        //删除集合 操作自定义数据权限
        await sysDataAuthorityCustomRepository.DeleteAsync(w => w.SysDataAuthorityId == sysDataAuthority.Id);
        foreach (var item in form.SysDataAuthorityCustomList)
        {
            item.SysDataAuthorityId = sysDataAuthority.Id;
        }

        await sysDataAuthorityCustomRepository.InsertRangeAsync(form.SysDataAuthorityCustomList);
    }

    /// <summary>
    /// 根据角色 id 获取数据权限
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, object>> GetDataAuthorityByRoleIdAsync(Guid roleId)
    {
        var result = new Dictionary<string, object>();

        var sysDataAuthority = await Repository.FindAsync(w => w.RoleId == roleId);
        sysDataAuthority = sysDataAuthority.NullSafe();
        var sysDataAuthorityCustomList = await sysDataAuthorityCustomRepository.Select
            .Where(w => w.SysDataAuthorityId == sysDataAuthority.Id)
            .ToListAsync();

        result.Add("sysDataAuthority", sysDataAuthority);
        result.Add("sysDataAuthorityCustomList", sysDataAuthorityCustomList);
        return result;
    }
}