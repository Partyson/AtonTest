using System.ComponentModel.DataAnnotations;
using AtonTest.Dto;
using FluentValidation;
using Mapster;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace AtonTest.Validators;

public class UserDataValidator
{
    private readonly UserInfoValidator userInfoValidator;
    private readonly LoginValidator loginValidator;
    private readonly PasswordValidator passwordValidator;

    public UserDataValidator(UserInfoValidator userInfoValidator, LoginValidator loginValidator, PasswordValidator passwordValidator)
    {
        this.userInfoValidator = userInfoValidator;
        this.loginValidator = loginValidator;
        this.passwordValidator = passwordValidator;
    }

    public async Task<ValidationResult> ValidateLoginAsync(string login)
    {
        var validationResult = await loginValidator.ValidateAsync(login);
        return validationResult;
    }
    
    public async Task<ValidationResult> ValidatePasswordAsync(string password)
    {
        var validationResult = await passwordValidator.ValidateAsync(password);
        return validationResult;
    }
    
    public async Task<ValidationResult> ValidateUserInfoAsync(UserInfoDto userInfo)
    {
        var validationResult = await userInfoValidator.ValidateAsync(userInfo);
        return validationResult;
    }

    public async Task<ValidationResult> ValidateCreateUserDtoAsync(CreateUserDto newUser)
    {
        var loginValidationResult = await loginValidator.ValidateAsync(newUser.Login);
        var passwordValidationResult = await passwordValidator.ValidateAsync(newUser.Password);
        var userInfoValidationResult = await userInfoValidator.ValidateAsync(newUser.Adapt<UserInfoDto>());
        
        var allFailures = new List<FluentValidation.Results.ValidationFailure>();
    
        if (!loginValidationResult.IsValid)
            allFailures.AddRange(loginValidationResult.Errors);

        if (!passwordValidationResult.IsValid)
            allFailures.AddRange(passwordValidationResult.Errors);

        if (!userInfoValidationResult.IsValid)
            allFailures.AddRange(userInfoValidationResult.Errors);

        return new ValidationResult(allFailures);
    }
}