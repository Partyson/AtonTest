using System.Net;
using System.Security.Claims;
using AtonTest.CustomAttributes;
using AtonTest.Dto;
using AtonTest.Helpers;
using AtonTest.Interfaces;
using AtonTest.Validators;
using EntityFrameworkCore.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtonTest.Controllers;

[ApiController]
public class UsersController : ControllerBase
{

    private readonly IUsersService usersService;
    private readonly IUnitOfWork unitOfWork;
    private readonly ILoginUniquenessValidator loginUniquenessValidator;
    private readonly UserDataValidator userDataValidator;

    public UsersController(IUsersService usersService, IUnitOfWork unitOfWork,
        ILoginUniquenessValidator loginUniquenessValidator, UserDataValidator userDataValidator)
    {
        this.usersService = usersService;
        this.unitOfWork = unitOfWork;
        this.loginUniquenessValidator = loginUniquenessValidator;
        this.userDataValidator = userDataValidator;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto user)
    {
        var validationResult = await userDataValidator.ValidateCreateUserDtoAsync(user);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.ToString(Environment.NewLine));

        var isUniqueLogin = await loginUniquenessValidator.IsLoginUniqueAsync(user.Login);
        if (!isUniqueLogin)
            return BadRequest("Пользователь с таким логином уже существует");
        
        var newUserId = await usersService.CreateUser(user, User.FindFirstValue("Login"));
        await unitOfWork.SaveChangesAsync();
        return Ok(newUserId);
    }

    [Authorize]
    [HttpPatch("modify-user-info/{modifiedUserLogin}")]
    [RequireOwnershipOrAdmin("modifiedUserLogin")]
    public async Task<IActionResult> ModifyUserInfo([FromBody] UserInfoDto newUserInfo,
        [FromRoute] string modifiedUserLogin)
    {
        var validationResult = await userDataValidator.ValidateUserInfoAsync(newUserInfo);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.ToString(Environment.NewLine));
        
        var modifiedUserId = await usersService.ModifyUserInfo(newUserInfo, modifiedUserLogin,
            User.FindFirstValue("Login"));
        if (modifiedUserId == Guid.Empty)
            return NotFound("Пользователя с таким логином не существует.");
        await unitOfWork.SaveChangesAsync();
        return Ok(modifiedUserId);
    }

    [Authorize]
    [HttpPatch("modify-user-password/{modifiedUserLogin}")]
    [RequireOwnershipOrAdmin("modifiedUserLogin")]
    public async Task<IActionResult> ModifyPassword([FromBody] string newPassword, [FromRoute] string modifiedUserLogin)
    {
        var validationResult = await userDataValidator.ValidatePasswordAsync(newPassword);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.ToString(Environment.NewLine));
        
        var modifiedUserId = await usersService.ModifyUserPassword(newPassword, modifiedUserLogin,
            User.FindFirstValue("Login"));
        if (modifiedUserId == Guid.Empty)
            return NotFound("Пользователя с таким логином не существует.");
        
        await unitOfWork.SaveChangesAsync();
        return Ok(modifiedUserId);
    }
    
    [Authorize]
    [HttpPatch("modify-user-login/{modifiedUserLogin}")]
    [RequireOwnershipOrAdmin("modifiedUserLogin")]
    public async Task<IActionResult> ModifyLogin([FromBody] string newLogin, [FromRoute] string modifiedUserLogin)
    {
        var validationResult = await userDataValidator.ValidateLoginAsync(newLogin);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.ToString(Environment.NewLine));
        
        var isUniqueLogin = await loginUniquenessValidator.IsLoginUniqueAsync(newLogin);
        if (!isUniqueLogin)
            return BadRequest("Пользователь с таким логином уже существует");
        
        var modifiedUserId = await usersService.ModifyUserLogin(newLogin, modifiedUserLogin,
            User.FindFirstValue("Login"));
        if (modifiedUserId == Guid.Empty)
            return NotFound("Пользователя с таким логином не существует.");
        await unitOfWork.SaveChangesAsync();
        return Ok(modifiedUserId);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("get-all-active-users")]
    public async Task<IActionResult> GetAllActiveUsers()
    {
        var users = await usersService.GetAllActiveUsers();
        return users.Count == 0 ? NotFound("Нет активных пользователей.") :
            Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("get-user/{login}")]
    public async Task<IActionResult> GetByLogin([FromRoute] string login)
    {
        var user = await usersService.GetUserByLogin(login);
        return user == null ? NotFound("Пользователя с таким логином не существует.") :
            Ok(user);
    }
    
    [HttpGet("get-my-user-info/{login}")]
    [RequireOwners("login")]
    public async Task<IActionResult> GetUserByLoginAndPassword([FromRoute] string login,
        [FromQuery] string password)
    {
        var user = await usersService.GetUserByLoginAndPassword(login, password);
        return user == null ? NotFound("Неверный пароль.") :
            Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("get-all-user-older-than")]
    public async Task<IActionResult> GetUsersOlderThan([FromQuery] int age)
    {
        var users = await usersService.GetUsersOlderThan(age);
        return users.Count == 0 ? NotFound($"Нет пользователей старше {age} лет.") :
            Ok(users);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("delete-user/{login}")]
    public async Task<IActionResult> DeleteUser([FromRoute] string login)
    {
        var user = await usersService.DeleteUserByLogin(login, User.FindFirstValue("Login"));
        if (user == Guid.Empty)
            return NotFound("Пользователя с таким логином не существует.");
        await unitOfWork.SaveChangesAsync();
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("restore-user/{login}")]
    public async Task<IActionResult> RestoreUser([FromRoute] string login)
    {
        var user = await usersService.RestoreUser(login, User.FindFirstValue("Login"));
        if (user == Guid.Empty)
            return NotFound("Пользователя с таким логином не существует.");
        await unitOfWork.SaveChangesAsync();
        return Ok(user);
    }
}