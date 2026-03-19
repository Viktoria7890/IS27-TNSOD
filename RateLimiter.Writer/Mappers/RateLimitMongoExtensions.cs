using RateLimiter.Writer.Data.Repositories.DbModels;
using RateLimiter.Writer.Domain.Models;

namespace RateLimiter.Writer.Mappers;

public static class RateLimitMongoExtensions
{
    public static RateLimitDbo ToDbo(this RateLimit x) =>
        new() { Route = x.Route, RequestsPerMinute = x.RequestsPerMinute };

    public static RateLimit ToDomain(this RateLimitDbo x) =>
        new() { Route = x.Route, RequestsPerMinute = x.RequestsPerMinute };
}