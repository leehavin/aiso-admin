using AiSo.Admin.Repository;
using AiSo.Admin.Service.Dtos.JwtToken;
using AiSo.Admin.Service.Dtos.User;
using AiUo;
using AiUo.Data.SqlSugar;
using AiUo.Net;
using AiUo.Security;
using AiUo.Text;

namespace AiSo.Admin.Service;

public class AccountService
{
    /// <summary>
    /// 注册用户
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<bool> Register(RegisterRequest request)
    {
        var db = DbUtil.GetDbById();
        var checkUser = db.Queryable<SysUserEntity>().FirstAsync(w => w.LoginName == request.UserName);
        if (checkUser != null)
        {
            return false;
        }

        var passwordSalt = SecurityUtil.GetPasswordSalt();
        var password = SecurityUtil.EncryptPassword(request.Password, passwordSalt);
        var user = new SysUserEntity
        {
            Id = ObjectId.NewId(),
            IsDeleted = false,
            LoginName = request.UserName,
            Password = password,
            PasswordSalt = passwordSalt
        };

        return await db.Insertable(user).ExecuteCommandAsync() > 0;
    }


    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="loginDto"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    public async Task<LoginResponse> Login(LoginRequest loginDto)
    {
        var user = await DbUtil.GetDbById().Queryable<SysUserEntity>().FirstAsync(c => c.LoginName == loginDto.Username);

        var isMatch = SecurityUtil.ValidatePassword(loginDto.Password, user.Password, user.PasswordSalt);

        if (!isMatch)
            throw new CustomException("LOGIN_ERROR", "密码错误");

        var jwtToken = JwtUtil.CreateJwtToken(user.Id, string.Empty, string.Empty);
        return new LoginResponse()
        {
            UserId = user.Id,
            Nickname = user.Nickname,
            Username = user.Name,
            Email = user.Email,
            Mobile = user.Mobile,
            JwtToken = jwtToken
        };
    }

    /// <summary>
    /// 刷新Token
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    public async Task<string> RefreshToken(RefreshJwtTokenRequest dto)
    {
        if (string.IsNullOrEmpty(dto.JwtToken))
            new Exception("JwtToken不能为空");

        if (dto.UserId is 0L)
            new Exception("UserId不能为空");

        var jwt = JwtUtil.ReadJwtToken(dto.JwtToken, string.Empty);
        if (jwt.Status == JwtTokenStatus.Invalid || jwt.UserId != dto.UserId.ToString())
            throw new CustomException(GResponseCodes.G_JWT_TOKEN_INVALID, "无效的token");

        if (jwt.Status == JwtTokenStatus.Expired)
            throw new CustomException(GResponseCodes.G_JWT_TOKEN_EXPIRED, "过期的token");

        var user = await DbUtil.GetDbById().Queryable<SysUserEntity>().FirstAsync(f => f.Id == dto.UserId);
        if (user == null)
            throw new CustomException(GResponseCodes.G_JWT_TOKEN_INVALID, "无效的用户");

        return JwtUtil.CreateJwtToken(dto.UserId, jwt.RoleString, string.Empty);
    }
}
