using AiSo.Admin.Service;
using AiSo.Admin.Service.Dtos.User;
using AiUo.AspNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AiSo.Admin.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<LoginResponseDto> Login(LoginRequestDto loginDto)
        {
            return await accountSvc.Login(loginDto);
        }
    }
}
