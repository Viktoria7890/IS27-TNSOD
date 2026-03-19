using MongoDB.Driver;
using RateLimiter.Writer.Data.Repositories.DbModels;
using RateLimiter.Writer.Domain.Exception;
using RateLimiter.Writer.Domain.Models;
using RateLimiter.Writer.Mappers;

namespace RateLimiter.Writer.Data.Repositories;

public sealed class RateLimitRepository : IRateLimitRepository
{
    private readonly IMongoCollection<RateLimitDbo> _col;

    public RateLimitRepository(IMongoDatabase db)
        => _col = db.GetCollection<RateLimitDbo>("rate_limits");

    public async Task EnsureIndexesAsync(CancellationToken ct)
    {
        var keys = Builders<RateLimitDbo>.IndexKeys.Ascending(x => x.Route);
        var model = new CreateIndexModel<RateLimitDbo>(keys, new CreateIndexOptions
        {
            Unique = true,
            Name = "ux_route"
        });
        await _col.Indexes.CreateOneAsync(model, cancellationToken: ct);
    }

    public async Task InsertAsync(RateLimit limit, CancellationToken ct)
        => await _col.InsertOneAsync(limit.ToDbo(), cancellationToken: ct);

    public async Task<RateLimit?> GetByRouteAsync(string route, CancellationToken ct)
    {
        var dbo = await _col.Find(x => x.Route == route).FirstOrDefaultAsync(ct);
        return dbo?.ToDomain();
    }

    public async Task UpdateAsync(RateLimit limit, CancellationToken ct)
    {
        var upd = Builders<RateLimitDbo>.Update.Set(x => x.RequestsPerMinute, limit.RequestsPerMinute);
        var res = await _col.UpdateOneAsync(x => x.Route == limit.Route, upd, cancellationToken: ct);

        if (res.MatchedCount == 0)
            throw new LimitNotFoundException(limit.Route);
    }

    public async Task DeleteAsync(string route, CancellationToken ct)
    {
        var res = await _col.DeleteOneAsync(x => x.Route == route, ct);
        if (res.DeletedCount == 0)
            throw new LimitNotFoundException(route);
    }
}