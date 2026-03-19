using Riok.Mapperly.Abstractions;
using RateLimiter.Writer.Domain.Models;

namespace RateLimiter.Writer.Mappers;

[Mapper]
public partial class RateLimitMapper
{
    [MapProperty(nameof(RateLimitDto.RequestPerMinute), nameof(RateLimit.RequestsPerMinute))]
    public partial RateLimit ToDomain(RateLimitDto dto);

    [MapProperty(nameof(RateLimit.RequestsPerMinute), nameof(RateLimitDto.RequestPerMinute))]
    public partial RateLimitDto ToDto(RateLimit model);
}