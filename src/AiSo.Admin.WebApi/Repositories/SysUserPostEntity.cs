using SqlSugar;

namespace AiSo.Admin.WebApi.Repositories;

/// <summary>
/// 用户鱼岗位绑定表
/// </summary>
[SugarTable("sys_user_post")]
public class SysUserPostEntity 
{
    /// <summary>
    /// 账户Id
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// 岗位Id
    /// </summary>
    public Guid PostId { get; set; }
}
