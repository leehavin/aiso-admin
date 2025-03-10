using AiUo.AspNet;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AiSo.Admin.WebApi.Controllers;

/// <summary>
/// 用户Apis
/// </summary>
[EnableCors()]
public class UserController : AiUoControllerBase
{
    /// <summary>
    /// 获取用户基本信息
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<string> GetUserInfo()
    {
        return null;
    }
}