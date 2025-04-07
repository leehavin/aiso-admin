namespace AiSo.Admin.WebApi.Models.Dtos.JwtToken;

public class RefreshJwtTokenRequest
{
    /// <summary>
    /// Token
    /// </summary>
    public string JwtToken { get; set; }

    /// <summary>
    /// 用户主键
    /// </summary>
    public string UserId { get; set; }
}
