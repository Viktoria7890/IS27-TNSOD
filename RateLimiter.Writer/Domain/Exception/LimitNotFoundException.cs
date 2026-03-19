namespace RateLimiter.Writer.Domain.Exception;

public sealed class LimitNotFoundException : System.Exception
{
    public LimitNotFoundException(string route)
        : base($"Rate limit for route '{route}' was not found.") { }
}