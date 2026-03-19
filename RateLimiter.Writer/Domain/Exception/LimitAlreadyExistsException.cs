namespace RateLimiter.Writer.Domain.Exception;
public sealed class LimitAlreadyExistsException : System.Exception
{
    public LimitAlreadyExistsException(string route)
        : base($"Rate limit for route '{route}' already exists.") { }
}