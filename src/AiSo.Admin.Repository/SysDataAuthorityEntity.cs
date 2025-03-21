﻿using SqlSugar;

namespace AiSo.Admin.Repository;

/// <summary>
/// 数据权限主表
/// </summary>
[SugarTable("sys_data_authority")]
public class SysDataAuthorityEntity 
{

    /// <summary>
    ///  PermissionType => 备注: 数据权限类型
    /// </summary>
    public SysDataAuthorityPermissionTypeEnum PermissionType { get; set; } = SysDataAuthorityPermissionTypeEnum.Custom;


    /// <summary>
    ///  RoleId => 备注: 角色Id
    /// </summary>
    public Guid RoleId { get; set; }


}

/// <summary>
/// 数据权限类型
/// </summary>
public enum SysDataAuthorityPermissionTypeEnum
{

    /// <summary>
    /// 自定义权限 
    /// </summary>
    Custom = 1,

    /// <summary>
    /// 查看所有数据 
    /// </summary>
    All,

    /// <summary>
    /// 仅查看本组织 
    /// </summary>
    Organization,

    /// <summary>
    /// 仅查看本组织和下属组织 
    /// </summary>
    OrganizationOrBranch,

    /// <summary>
    /// 仅查看本人 
    /// </summary>
    Self
}