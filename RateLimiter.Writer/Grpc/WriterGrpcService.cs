using FluentValidation;
using Grpc.Core;
using RateLimiter.Writer.Domain.Exception;
using RateLimiter.Writer.Domain.Models;
using RateLimiter.Writer.Domain.Services;
using RateLimiter.Writer.Mappers;

namespace RateLimiter.Writer.Grpc;

using GrpcWriter = RateLimiter.Writer.Writer;

public sealed class WriterGrpcService: GrpcWriter.WriterBase
{
    private readonly IRateLimitService _service;
    private readonly RateLimitMapper _mapper;

    public WriterGrpcService(IRateLimitService service, RateLimitMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public override async Task<RateLimitResponse> CreateLimit(CreateLimitRequest request, ServerCallContext context)
    {
        var ct = context.CancellationToken;
        var domain = _mapper.ToDomain(request.Limit);
        var created = await _service.CreateAsync(domain, ct);
        return new RateLimitResponse { Limit = _mapper.ToDto(created) };
    }
    
    public override async Task<RateLimitResponse> GetLimitByRoute(GetLimitByRouteRequest request, ServerCallContext context)
    {
        var ct = context.CancellationToken;
        var found = await _service.GetByRouteAsync(request.Route, ct);
        return new RateLimitResponse { Limit = _mapper.ToDto(found) };
    }

    public override async Task<RateLimitResponse> UpdateLimit(UpdateLimitRequest request, ServerCallContext context)
    {
        var ct = context.CancellationToken;
        var domain = _mapper.ToDomain(request.Limit);
        var updated = await _service.UpdateAsync(domain, ct);
        return new RateLimitResponse { Limit = _mapper.ToDto(updated) };
    }

    public override async Task<Empty> DeleteLimit(DeleteLimitRequest request, ServerCallContext context)
    {
        var ct = context.CancellationToken;
        await _service.DeleteAsync(request.Route, ct);
        return new Empty();
    }
}