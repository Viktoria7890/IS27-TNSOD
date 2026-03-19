using RateLimiter.Writer.Domain.Models;

namespace RateLimiter.Writer.Data.Repositories;

public interface IRateLimitRepository
{
    Task EnsureIndexesAsync(CancellationToken ct);
    Task InsertAsync(RateLimit limit, CancellationToken ct);
    Task<RateLimit?> GetByRouteAsync(string route, CancellationToken ct);
    Task UpdateAsync(RateLimit limit, CancellationToken ct);
    Task DeleteAsync(string route, CancellationToken ct);
}