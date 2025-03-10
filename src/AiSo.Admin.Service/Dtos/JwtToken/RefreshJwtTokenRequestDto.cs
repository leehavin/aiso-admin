namespace AiSo.Admin.Service.Dtos.JwtToken;

public class RefreshJwtTokenRequestDto
{
    /// <summary>
    /// Token
    /// </summary>
    public string JwtToken { get; set; }

    /// <summary>
    /// 用户主键
    /// </summary>
    public long UserId { get; set; }
}
