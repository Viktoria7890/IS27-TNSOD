using Riok.Mapperly.Abstractions;
using DomainRateLimit = ReaderService.Domain.Models.RateLimit;
using DtoRateLimit = RateLimiter.Reader.RateLimitDto;

namespace RateLimiter.Reader.Mappers;

[Mapper]
public partial class RateLimitMapper
{ 
    public partial DtoRateLimit ToDto(DomainRateLimit model);
}