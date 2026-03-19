using RateLimiter.Writer.Domain.Models;
namespace RateLimiter.Writer.Domain.Services;

public interface IRateLimitService
{
    Task<RateLimit> CreateAsync(RateLimit limit, CancellationToken ct);
    Task<RateLimit> GetByRouteAsync(string route, CancellationToken ct);
    Task<RateLimit> UpdateAsync(RateLimit limit, CancellationToken ct);
    Task DeleteAsync(string route, CancellationToken ct);
}
