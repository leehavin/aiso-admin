namespace AiSo.Admin.WebApi.Repositories;

/// <summary>
/// 本地化
/// </summary>
public class SysLocaleEntity 
{
    /// <summary>
    /// 键
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 值
    /// </summary>
    public string? Value { get; set; }
}
