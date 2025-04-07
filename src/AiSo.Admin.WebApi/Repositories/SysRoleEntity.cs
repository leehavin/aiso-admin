using SqlSugar;

namespace AiSo.Admin.WebApi.Repositories;

/// <summary>
/// 角色
/// </summary>
[SugarTable("sys_role")]
public class SysRoleEntity 
{
    /// <summary>
    /// 编号
    /// </summary>
    public int? Number { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 是否是管理员
    /// </summary>
    public bool IsAdmin { get; set; } = false;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 删除锁 ，如果是 true 则不能删除
    /// </summary>
    public bool DeleteLock { get; set; } = false;
}