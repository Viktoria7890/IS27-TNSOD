using RateLimiter.Writer.Domain.Exception;
using RateLimiter.Writer.Domain.Models;
using RateLimiter.Domain.Validators;

namespace RateLimiter.Writer.Domain.Services;

using FluentValidation;
using RateLimiter.Writer.Data.Repositories;     // интерфейс репозитория из Data-слоя


public sealed class RateLimitService : IRateLimitService
{
    private readonly IRateLimitRepository _repo;
    private readonly CreateRateLimitValidator _createValidator;
    private readonly UpdateRateLimitValidator _updateValidator;

    public RateLimitService(
        IRateLimitRepository repo,
        CreateRateLimitValidator createValidator,
        UpdateRateLimitValidator updateValidator)
    {
        _repo = repo;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<RateLimit> CreateAsync(RateLimit limit, CancellationToken ct)
    {
        _createValidator.ValidateAndThrow(limit);

        var existing = await _repo.GetByRouteAsync(limit.Route, ct);
        if (existing is not null)
            throw new LimitAlreadyExistsException(limit.Route);

        await _repo.InsertAsync(limit, ct);
        return limit;
    }

    public async Task<RateLimit> GetByRouteAsync(string route, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(route))
            throw new ValidationException("Route is required.");

        var found = await _repo.GetByRouteAsync(route, ct);
        if (found is null)
            throw new LimitNotFoundException(route);

        return found;
    }

    public async Task<RateLimit> UpdateAsync(RateLimit limit, CancellationToken ct)
    {
        _updateValidator.ValidateAndThrow(limit);

        var existing = await _repo.GetByRouteAsync(limit.Route, ct);
        if (existing is null)
            throw new LimitNotFoundException(limit.Route);

        await _repo.UpdateAsync(limit, ct);
        return limit;
    }

    public async Task DeleteAsync(string route, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(route))
            throw new ValidationException("Route is required.");

        var existing = await _repo.GetByRouteAsync(route, ct);
        if (existing is null)
            throw new LimitNotFoundException(route);

        await _repo.DeleteAsync(route, ct);
    }
}
