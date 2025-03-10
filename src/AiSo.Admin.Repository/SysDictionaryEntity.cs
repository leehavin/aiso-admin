using SqlSugar;

namespace AiSo.Admin.Repository;

/// <summary>
/// 数据字典
/// </summary>
[SugarTable("sys_dictionary")]
public class SysDictionaryEntity 
{
    /// <summary>
    /// 排序号
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 分组名称/键
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// 父级Id
    /// </summary>
    public int? ParentId { get; set; }


}
