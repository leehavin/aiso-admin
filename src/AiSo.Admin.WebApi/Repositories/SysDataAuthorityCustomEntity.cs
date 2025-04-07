using SqlSugar;

namespace AiSo.Admin.WebApi.Repositories;

/// <summary>
/// 数据权限子表
/// </summary>
[SugarTable("sys_data_authority_custom")]
public class SysDataAuthorityCustomEntity 
{

    /// <summary>
    ///  SysDataAuthorityId => 备注: 数据权限主表Id
    /// </summary>
    public Guid? SysDataAuthorityId { get; set; }


    /// <summary>
    ///  SysOrganizationId => 备注: 组织Id
    /// </summary>
    public int SysOrganizationId { get; set; }


}