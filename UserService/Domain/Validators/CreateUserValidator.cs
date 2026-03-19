using FluentValidation;
using UserService.Domain.Models;

namespace UserService.Domain.Validators;

public class CreateUserValidator : AbstractValidator<User>
{
    public CreateUserValidator()
    {
        RuleFor(u => u.Login).NotEmpty().MinimumLength(3);
        RuleFor(u => u.Password).NotEmpty().MinimumLength(6);
        RuleFor(u => u.Name).NotEmpty();
        RuleFor(u => u.Surname).NotEmpty();
        RuleFor(u => u.Age).InclusiveBetween(14, 120);
    }
}