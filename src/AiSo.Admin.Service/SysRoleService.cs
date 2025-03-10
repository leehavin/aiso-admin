namespace AiSo.Admin.Service;

/// <summary>
/// 角色服务
/// </summary>
public class SysRoleService(
    IServiceProvider serviceProvider,
    IRepository<SysUserRole> sysUserRoleRepository,
    IRepository<SysDataAuthority> sysDataAuthorityRepository,
    IRepository<SysDataAuthorityCustom> sysDataAuthorityCustomRepository) : ApplicationService<SysRole, Guid, SysRole, SysRole>(serviceProvider)
{
    /// <summary>
    /// 获取列表数据
    /// </summary>
    /// <param name="pagingSearchInput"></param>
    /// <returns></returns>
    public override async Task<PagingView> FindListAsync(PagingSearchInput<SysRole> pagingSearchInput)
    {
        var query = (from sysRole in Repository.Select
                    from sysDataAuthority in sysDataAuthorityRepository.Select
                        .Where(w => w.RoleId == sysRole.Id)
                        .DefaultIfEmpty()
                    select new
                    {
                        t1 = sysRole,
                        t2 = sysDataAuthority
                    })
                .WhereIf(!string.IsNullOrWhiteSpace(pagingSearchInput.Search?.Name),
                    a => a.t1.Name.Contains(pagingSearchInput.Search.Name))
                .OrderBy(w => w.t1.Number)
                .Select(w => new
                {
                    w.t1.Number,
                    w.t1.Name,
                    w.t1.Remark,
                    w.t1.DeleteLock,
                    PermissionType = w.t2 == null ? 0 : w.t2.PermissionType,
                    w.t1.LastModificationTime,
                    w.t1.CreationTime,
                    w.t1.Id,
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
    /// 根据id数组删除
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public override async Task DeleteListAsync(List<Guid> ids)
    {
        foreach (var item in ids)
        {
            var role = await Repository.FindByIdAsync(item);
            if (role.DeleteLock) throw MessageBox.Show("该信息已被锁定不能删除！");
            await Repository.DeleteAsync(role);
            await sysUserRoleRepository.DeleteAsync(w => w.RoleId == item);
            var list = await sysDataAuthorityRepository.ToListAsync(w => w.RoleId == item);
            await sysDataAuthorityCustomRepository.DeleteAsync(w =>
                list.Select(w => w.Id).Contains(w.SysDataAuthorityId.Value));
            await sysDataAuthorityRepository.DeleteAsync(list);
        }
    }

    /// <summary>
    /// 查询表单数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public override async Task<Dictionary<string, object?>> FindFormAsync(Guid id)
    {
        var res = new Dictionary<string, object?>();
        var form = await Repository.FindByIdAsync(id);
        form = form.NullSafe();

        if (id == Guid.Empty)
        {
            var maxNum = await Repository.Select.MaxAsync(w => w.Number);
            form.Number = maxNum + 1;
        }

        res[nameof(id)] = id == Guid.Empty ? "" : id;
        res[nameof(form)] = form;
        return res;
    }
}