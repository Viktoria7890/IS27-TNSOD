using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using RateLimiter.Writer.Domain.Exception;

namespace RateLimiter.Writer.Grpc;

public sealed class ExceptionMappingInterceptor : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (ValidationException e)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, e.Message));
        }
        catch (LimitAlreadyExistsException e)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, e.Message));
        }
        catch (LimitNotFoundException e)
        {
            throw new RpcException(new Status(StatusCode.NotFound, e.Message));
        }
    }
}