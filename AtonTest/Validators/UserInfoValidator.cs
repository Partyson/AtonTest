using AtonTest.Dto;
using FluentValidation;

namespace AtonTest.Validators;

public class UserInfoValidator : AbstractValidator<UserInfoDto>
{
    public UserInfoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Имя обязательно.")
            .Matches("^[a-zA-Zа-яА-ЯёЁ]+$").WithMessage("Имя может содержать только буквы (латиница или кириллица).");

        RuleFor(x => x.Gender)
            .InclusiveBetween(0, 2).WithMessage("Пол должен быть 0 (женщина), 1 (мужчина) или 2 (неизвестно).");

        RuleFor(x => x.Birthday)
            .LessThanOrEqualTo(DateTime.Today)
            .WithMessage("Дата рождения не может быть в будущем.")
            .When(x => x.Birthday.HasValue);
    }
}