using SqlSugar;

namespace AiSo.Admin.Repository;

/// <summary>
/// 菜单与功能绑定
/// </summary>
[SugarTable("sys_menu_function")]
public class SysMenuFunctionEntity 
{
    public long Id { get; set; }

    /// <summary>
    /// 序号
    /// </summary>
    /// <value></value>
    public int? Number { get; set; }

    /// <summary>
    /// 菜单id
    /// </summary>
    /// <value></value>
    public int MenuId { get; set; }

    /// <summary>
    /// 功能编码
    /// </summary>
    /// <value></value>
    public string? FunctionCode { get; set; }

    /// <summary>
    /// 功能名称
    /// </summary>
    /// <value></value>
    public string? FunctionName { get; set; }

    /// <summary>
    /// 备注
    /// </summary>
    /// <value></value>
    public string? Remark { get; set; }
}
