using System.ComponentModel.DataAnnotations;
using SqlSugar;

namespace AiSo.Admin.Repository;

/// <summary>
/// 系统账号
/// </summary>
[SugarTable("sys_user")]
public partial class SysUserEntity 
{
    /// <summary>
    /// 主键
    /// </summary>
    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public string Id { get; set; }

    /// <summary>
    /// 账户名称
    /// </summary>
    [Required(ErrorMessage = "用户名称不能为空!")]
    [SugarColumn(ColumnName = "Name")]
    public string? Name { get; set; }

    /// <summary>
    /// 登录账号
    /// </summary>
    [Required(ErrorMessage = "登录名称不能为空!")]
    [SugarColumn(ColumnName = "LoginName")]
    public string? LoginName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    [SugarColumn(ColumnName = "Password")]
    public string? Password { get; set; }


    [SugarColumn(ColumnName = "NickName")]
    public string Nickname { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [SugarColumn(ColumnName = "Mobile")]
    public string? Mobile { get; set; }

    /// <summary>
    /// 账户邮件地址
    /// </summary>
    [Required(ErrorMessage = "邮件不能为空!"), EmailAddress(ErrorMessage = "邮件格式不正确!")]
    [SugarColumn(ColumnName = "Email")]
    public string? Email { get; set; }


    [SugarColumn(ColumnName = "PasswordSalt")]
    public string PasswordSalt { get; set; }

    /// <summary>
    /// 删除锁 ，如果是 true 则不能删除
    /// </summary>
    [SugarColumn(ColumnName = "IsDeleted")]
    public bool IsDeleted { get; set; } = false;
}