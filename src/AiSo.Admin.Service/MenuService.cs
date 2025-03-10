using AiSo.Admin.Repository;
using AiUo.Data.SqlSugar;

namespace AiSo.Admin.Service;

public class MenuService
{

    /// <summary>
    /// 根据角色获取用户菜单
    /// </summary>
    /// <returns></returns>
    public async Task<List<SysMenuEntity>> GetMenusByCurrentRoleAsync()
    {
        var sysMenuAllList = await DbUtil.GetDbById().Queryable<SysMenuEntity>().Where(f => f.State).OrderBy(f => f.Number).ToListAsync();

        // TODO 判断当前是否为管理员
        // if (_accountInfo.IsAdministrator) return sysMenuAllList;

        //var sysMenuList = await (
        //            from t1 in sysRoleMenuFunctionRepository.Select.Where(w =>
        //                _accountInfo.SysRoles.Select(s => s.Id).Contains(w.RoleId))
        //            from t2 in Repository.Select.Where(w => w.Id == t1.MenuId && w.State)
        //                //.DefaultIfEmpty()
        //            from t3 in sysMenuFunctionRepository.Select
        //                .Where(w => w.Id == t1.MenuFunctionId &&
        //                            w.FunctionCode == PermissionFunctionConsts.Function_Display && t2.Id == w.MenuId)
        //                //.DefaultIfEmpty()
        //            select t2
        //        )
        //        .ToListAsync();

        var newSysMenuList = new List<SysMenuEntity>();

        //foreach (var item in sysMenuList)
        //{
        //    //检查 item 是否已经存在于新集合中
        //    if (!newSysMenuList.Any(w => w.Id == item.Id))
        //        newSysMenuList.Add(item);

        //    CheckUpperLevel(sysMenuAllList, sysMenuList, newSysMenuList, item);
        //}

        return newSysMenuList.OrderBy(w => w.Number).ToList();
    }
}
