﻿using SqlSugar;

namespace AiSo.Admin.Repository;

/// <summary>
/// 菜单
/// </summary>
[SugarTable("sys_menu")]
public class SysMenuEntity 
{
    /// <summary>
    /// 级别码 1.1.1
    /// </summary>
    public string? LevelCode { get; set; }

    /// <summary>
    /// 编号
    /// </summary>
    public int? Number { get; set; }

    /// <summary>
    /// 菜单名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Vue组件名称
    /// </summary>
    public string? ComponentName { get; set; }

    /// <summary>
    /// 菜单物理路径
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 路由地址
    /// </summary>
    public string? Router { get; set; }

    /// <summary>
    /// 默认跳转地址
    /// </summary>
    public string? JumpUrl { get; set; }

    /// <summary>
    /// 菜单图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    public int? ParentId { get; set; }

    /// <summary>
    /// 显示状态 => 显示 | 隐藏
    /// </summary>
    public bool Show { get; set; } = true;

    /// <summary>
    /// 选项卡是否可关闭
    /// </summary>
    public bool Close { get; set; } = true;

    /// <summary>
    /// 是否缓存 => 是 | 否
    /// </summary>
    public bool KeepAlive { get; set; } = true;

    /// <summary>
    /// 菜单状态 => 正常 | 停用
    /// </summary>
    public bool State { get; set; } = true;

    /// <summary>
    /// 菜单类型
    /// </summary>
    /// <value></value>
    public SysMenuTypeEnum Type { get; set; } = SysMenuTypeEnum.菜单;

    /// <summary>
    /// 菜单模式
    /// </summary>
    public SysMenuModeEnum Mode { get; set; } = SysMenuModeEnum.普通;

    /// <summary>
    /// 模块地址 (微前端) Dev 开发模式
    /// </summary>
    public string? ModuleUrl { get; set; }

    /// <summary>
    /// 模块地址 (微前端) Pro 生产模式
    /// </summary>
    public string? ModuleUrlPro { get; set; }

}

public enum SysMenuTypeEnum
{
    目录 = 1,
    菜单
}

public enum SysMenuModeEnum
{
    普通 = 1,
    微前端
}