using FluentValidation;

namespace AtonTest.Validators;

public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(password => password)
            .NotEmpty().WithMessage("Пароль обязателен.")
            .Matches("^[a-zA-Z0-9]+$").WithMessage("Пароль может содержать только латинские буквы и цифры.");
    }
}