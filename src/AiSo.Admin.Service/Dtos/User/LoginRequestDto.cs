namespace AiSo.Admin.Service.Dtos.User;

public class LoginRequestDto
{
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
    /// 密码
    /// </summary>
    public string Password { get; set; }
}
