using RateLimiter.Writer.Domain.Models;
using FluentValidation;

namespace RateLimiter.Domain.Validators;

public sealed class UpdateRateLimitValidator : AbstractValidator<RateLimit>
{
    public UpdateRateLimitValidator()
    {
        RuleFor(x => x.Route)
            .NotEmpty().WithMessage("Route is required.")
            .MaximumLength(256);

        RuleFor(x => x.RequestsPerMinute)
            .GreaterThan(0).WithMessage("RequestsPerMinute must be > 0.")
            .LessThanOrEqualTo(1_000_000);
    }
}
