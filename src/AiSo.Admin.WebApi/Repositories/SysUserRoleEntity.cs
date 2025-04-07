using SqlSugar;

namespace AiSo.Admin.WebApi.Repositories;

/// <summary>
/// 用户与角色绑定
/// </summary>
[SugarTable("sys_user_role")]
public class SysUserRoleEntity 
{
    [SugarColumn(ColumnName = "UserId", IsPrimaryKey = true)]
    public Guid UserId { get; set; }
    
    [SugarColumn(ColumnName = "RoleId", IsPrimaryKey = true)]
    public Guid RoleId { get; set; }
}