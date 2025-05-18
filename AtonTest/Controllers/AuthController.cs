using AtonTest.Dto;
using AtonTest.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtonTest.Controllers;

[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUsersService usersService;

    public AuthController(IUsersService usersService)
    {
        this.usersService = usersService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CredentialsDto credentials)
    {
        var token = await usersService.LoginUser(credentials);
        if (token == null)
            return NotFound("Пользователя с таким логином и паролем не существует.");
        var cookieOptions = new CookieOptions
        {
            Expires = DateTime.UtcNow.AddHours(12),
            HttpOnly = true,
            Secure = false,
            MaxAge = TimeSpan.FromDays(7),
            SameSite = SameSiteMode.Strict
        };
        Response.Cookies.Append("token", token, cookieOptions);
        return Ok(token);
    }
    
    [Authorize]
    [HttpPost("logout")]
    public ActionResult Logout()
    {
        Response.Cookies.Delete("token");
        return Ok("Logged out");
    }
}