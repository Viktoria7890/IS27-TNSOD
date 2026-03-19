namespace RateLimiter.Reader.Domain.Sevices;
using ReaderService.Domain.Models;

public interface IRateLimitReader
{
    Task StartAsync();
    
    IReadOnlyList<RateLimit> GetAll();
}
