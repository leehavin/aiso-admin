using AiSo.Admin.Repository;
using AiUo.Data.SqlSugar;

namespace AiSo.Admin.Service;

public class MenuService
{

    /// <summary>
    /// 根据角色获取用户菜单
    /// </summary>
    /// <returns></returns>
    public async Task<List<SysMenuEntity>> GetMenusByCurrentRoleAsync(long userId)
    {
        var sysMenuAllList = await DbUtil.GetDbById().Queryable<SysMenuEntity>().Where(f => f.State).OrderBy(f => f.Number).ToListAsync();

        // TODO 判断当前是否为管理员
        // if (_accountInfo.IsAdministrator) return sysMenuAllList;
        var roleIds = new List<long>();

        var db = DbUtil.GetDbById();

        var menus = await db.Queryable<SysMenuEntity>()
            .InnerJoin<SysMenuFunctionEntity>((a, b) => a.Id == b.MenuId && a.State)
            .InnerJoin<SysRoleMenuFunctionEntity>((a, b, c) => a.Id == c.MenuId && c.MenuFunctionId == b.Id)
            .Where((a, b, c) => roleIds.Contains(c.RoleId))
            .Select((a, b, c) => a)
            .ToListAsync();

        var newSysMenuList = new List<SysMenuEntity>();

        foreach (var item in menus)
        {
            if (!newSysMenuList.Any(w => w.Id == item.Id))
                newSysMenuList.Add(item);
            CheckUpperLevel(sysMenuAllList, menus, newSysMenuList, item);
        }

        return [.. newSysMenuList.OrderBy(w => w.Number)];
    }

    private void CheckUpperLevel(List<SysMenuEntity> sysMenuAllList, List<SysMenuEntity> oldSysMenuList, List<SysMenuEntity> newSysMenuList, SysMenuEntity menu)
    {
        if (oldSysMenuList.Any(w => w.Id == menu.ParentId)) return;

        var item = sysMenuAllList.Find(w => w.Id == menu.ParentId);
        if (item == null) return;

        //检查 item 是否已经存在于新集合中
        if (!newSysMenuList.Any(w => w.Id == item.Id))
            newSysMenuList.Add(item);

        CheckUpperLevel(sysMenuAllList, oldSysMenuList, newSysMenuList, item);
    }
}
