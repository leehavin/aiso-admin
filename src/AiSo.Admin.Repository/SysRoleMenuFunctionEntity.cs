﻿using SqlSugar;

namespace AiSo.Admin.Repository;

/// <summary>
/// 角色菜单功能绑定
/// </summary>
[SugarTable("sys_role_menu_function")]
public class SysRoleMenuFunctionEntity 
{
    public long RoleId { get; set; }
    public int MenuId { get; set; }
    public long MenuFunctionId { get; set; }
}