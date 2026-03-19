using FluentValidation;
using UserService.Domain.Models;

namespace UserService.Domain.Validators;

public class UpdateUserValidator : AbstractValidator<User>
{
    public UpdateUserValidator()
    {
        RuleFor(u => u.Id).GreaterThan(0);
        RuleFor(u => u.Name).NotEmpty();
        RuleFor(u => u.Surname).NotEmpty();
        RuleFor(u => u.Age).InclusiveBetween(14, 120);
    }
}