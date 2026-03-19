using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using RateLimiter.Reader.Domain.Sevices;
using RateLimiter.Reader.Mappers;

namespace RateLimiter.Reader.Grpc;

public class ReaderGrpcService: Reader.ReaderBase
{
    private readonly IRateLimitReader _reader;
    private readonly RateLimitMapper _mapper;
    
    public ReaderGrpcService(IRateLimitReader reader, RateLimitMapper mapper)
    {
        _reader  = reader  ?? throw new ArgumentNullException(nameof(reader));
        _mapper  = mapper  ?? throw new ArgumentNullException(nameof(mapper));
    }

    public override Task<GetAllLimitsResponse> GetAllLimits(Empty request, ServerCallContext context)
    {
        var snapshot = _reader.GetAll();
        var resp = new GetAllLimitsResponse();
        foreach (var item in snapshot)
            resp.Limits.Add(_mapper.ToDto(item));

        return Task.FromResult(resp);
    }
}