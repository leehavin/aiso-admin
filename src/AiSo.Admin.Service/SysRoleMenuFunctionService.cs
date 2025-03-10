﻿namespace AiSo.Admin.Service;

/// <summary>
/// 角色 菜单 功能服务
/// </summary>
public class SysRoleMenuFunctionService(
    IServiceProvider serviceProvider,
    IRepository<SysMenu> sysMenuRepository,
    IRepository<SysMenuFunction> sysMenuFunctionRepository)
    : ApplicationService<SysRoleMenuFunction, Guid, SysRoleMenuFunction, List<SysRoleMenuFunctionFormDto>>(
        serviceProvider)
{
    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    [Transactional]
    public override async Task SaveFormAsync(List<SysRoleMenuFunctionFormDto> form)
    {
        var sysRoleMenuFunctions = new List<SysRoleMenuFunction>();
        var roleId = form.Count > 0 ? form[0].RoleId : default;

        if (roleId == Guid.Empty) return;

        await Repository.DeleteAsync(w => w.RoleId == roleId);

        foreach (var item in form)
        {
            var menuId = item.MenuId;
            var menuFuncionList = item.MenuFunctionIdList;

            var list = menuFuncionList
                .Select(item => new SysRoleMenuFunction { MenuId = menuId, RoleId = roleId, MenuFunctionId = item })
                .ToList();

            sysRoleMenuFunctions.AddRange(list);
        }

        await Repository.InsertRangeAsync(sysRoleMenuFunctions);
    }

    /// <summary>
    /// 获取 角色 勾选的菜单功能集合 
    /// </summary>
    /// <param name="roleId"></param>
    /// <returns></returns>
    public async Task<List<Dictionary<string, object>>> GetRoleMenuFunctionByRoleIdAsync(Guid roleId)
    {
        var allMenus = await sysMenuRepository.ToListAllAsync();
        var allMenuFunction = await sysMenuFunctionRepository.ToListAllAsync();
        var allRoleMenuFunction = await Repository.Select.Where(w => w.RoleId == roleId).ToListAsync();

        var result = new List<Dictionary<string, object>>();

        foreach (var item in allMenus)
        {
            //组装菜单
            var menuFunctions = allMenuFunction
                .Where(w => w.MenuId == item.Id)
                .Select(w => new { w.Id, Label = w.FunctionName })
                .ToList();

            //获取当前角色勾选该菜单的按钮数组
            var checkedMenuFunctionIds = allRoleMenuFunction
                    .Where(w => w.MenuId == item.Id)
                    .Select(w => w.MenuFunctionId)
                    .ToList()
                ;

            result.Add(new Dictionary<string, object>
            {
                ["id"] = item.Id,
                ["name"] = item.Name,
                ["number"] = item.Number,
                ["parentId"] = item.ParentId,
                ["levelCode"] = item.LevelCode,
                ["menuFunctions"] = menuFunctions,
                ["checkedMenuFunctionIds"] = checkedMenuFunctionIds,
                ["checkAll"] = menuFunctions.Count == checkedMenuFunctionIds.Count,
                ["indeterminate"] = checkedMenuFunctionIds.Count > 0 &&
                                    checkedMenuFunctionIds.Count < menuFunctions.Count
            });
        }

        return result;
    }
}