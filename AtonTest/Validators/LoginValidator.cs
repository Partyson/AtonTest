using FluentValidation;

namespace AtonTest.Validators;

public class LoginValidator : AbstractValidator<string>
{
    public LoginValidator()
    {
        RuleFor(login => login)
            .NotEmpty().WithMessage("Логин обязателен.")
            .Matches("^[a-zA-Z0-9]+$").WithMessage("Логин может содержать только латинские буквы и цифры.");
    }
}