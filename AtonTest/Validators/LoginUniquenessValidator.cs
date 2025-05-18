using AtonTest.Dto;
using AtonTest.Interfaces;

namespace AtonTest.Validators;

public class LoginUniquenessValidator : ILoginUniquenessValidator
{
    private readonly IUsersService usersService;

    public LoginUniquenessValidator(IUsersService usersService)
    {
        this.usersService = usersService;
    }

    public async Task<bool> IsLoginUniqueAsync(string login)
    {
        var userEntity = await usersService.GetUserByLogin(login);
        return userEntity == null;
    }
}