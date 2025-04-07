namespace AiSo.Admin.WebApi.Models.Dtos.User;

/// <summary>
/// 登录返回对象
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// 用户主键
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string Nickname { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Email
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    public string Mobile { get; set; }

    /// <summary>
    /// Token
    /// </summary>
    public string JwtToken { get; set; }
}
