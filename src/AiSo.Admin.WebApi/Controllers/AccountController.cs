using AiSo.Admin.Services;
using AiSo.Admin.WebApi.Models.Dtos.User;
using AiUo.AspNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AiSo.Admin.WebApi.Controllers;

/// <summary>
/// 账号Apis
/// </summary>
[EnableCors()]
[ClientSignFilter()]
public class AccountController : AiUoControllerBase
{
    private readonly AccountService accountSvc = new();
     

    /// <summary>
    /// 登录
    /// MobileNotExist - mobile不存在
    /// UserStatusError - 用户状态异常
    /// LoginErrorLimit - 登录错误次数过多，等待30分钟
    /// UserPasswordError - 登录密码错误
    /// UsernameInvalid
    /// </summary>
    /// <param name="ipo"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<LoginResponse> Login(LoginRequest loginDto)
    {
        return await accountSvc.Login(loginDto);
    }
     
    /// <summary>
    /// 修改密码
    /// UserPasswordError - 登录密码错误
    /// </summary>
    /// <param name="ipo"></param>
    //[HttpPost]
    //public async Task ChangePassword(ChangePasswordIpo ipo)
    //{
    //    if (ipo.UserId != UserId)
    //        throw new Exception("用户userId不匹配！");
    //    await _changeSvc.ChangePassword(ipo);
    //}

    /// <summary>
    /// 用户主动退出登录
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task Loginout()
    {
        await HttpContextEx.SignOutUseCookie();
    }
}
