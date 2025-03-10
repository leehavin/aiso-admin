using AiSo.Admin.Repository;
using AiSo.Admin.Service.Dtos.User;
using AiUo;
using AiUo.Data.SqlSugar;
using AiUo.Security;

namespace AiSo.Admin.Service;

public class AccountService
{

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="loginDto"></param>
    /// <returns></returns>
    /// <exception cref="CustomException"></exception>
    public async Task<LoginResponseDto> Login(LoginRequestDto loginDto)
    {
        var user = await DbUtil.GetDbById().Queryable<SysUserEntity>().FirstAsync(c => c.LoginName == loginDto.Username);

        var isMatch = SecurityUtil.ValidatePassword(loginDto.Password, user.Password, user.PasswordSalt);

        if (!isMatch)
            throw new CustomException("LOGIN_ERROR", "密码错误");


        var jwtToken = JwtUtil.CreateJwtToken(user.Id, string.Empty, string.Empty);

        return new LoginResponseDto()
        {
            UserId = user.Id,
            Nickname = user.Nickname,
            Username = user.Name,
            Email = user.Email,
            Mobile = user.Mobile,
            JwtToken = jwtToken
        };
    }
}
