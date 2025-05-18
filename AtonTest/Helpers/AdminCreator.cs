using AtonTest.Dto;
using AtonTest.Interfaces;
using EntityFrameworkCore.UnitOfWork.Interfaces;

namespace AtonTest.Helpers;

public static class AdminCreator
{
    public static async Task SeedAdminUserAsync(IUsersService userService, IUnitOfWork unitOfWork)
    {
        var existing = await userService.GetUserByLogin("admin");
        if (existing != null)
            return;

        var adminDto = new CreateUserDto
        {
            Login = "admin",
            Password = "admin",
            Name = "Администратор",
            Gender = 1,
            Birthday = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Admin = true
        };

        await userService.CreateUser(adminDto,"system");
        await unitOfWork.SaveChangesAsync();
    }
}