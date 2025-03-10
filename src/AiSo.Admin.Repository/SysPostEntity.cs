﻿using SqlSugar;

namespace AiSo.Admin.Repository;

/// <summary>
/// 岗位
/// </summary>
[SugarTable("sys_post")]
public class SysPostEntity 
{
    /// <summary>
    /// 编号
    /// </summary>
    public int? Number { get; set; }

    /// <summary>
    /// 岗位编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 岗位名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    public StateEnum State { get; set; } = StateEnum.正常;

    /// <summary>
    /// 备注
    /// </summary>
    public string? Remarks { get; set; }
}

/// <summary>
/// 状态 枚举
/// </summary>
public enum StateEnum
{
    正常 = 1,
    停用
}

