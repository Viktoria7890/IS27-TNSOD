using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;
using UserService.Domain.Exceptions;

namespace UserService.Grpc.Interceptors;

public sealed class GlobalExceptionInterceptor : Interceptor
{
    private readonly ILogger<GlobalExceptionInterceptor> _logger;

    public GlobalExceptionInterceptor(ILogger<GlobalExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (UserNotFoundException ex)
        {
            throw new RpcException(new Status(StatusCode.NotFound, ex.Message));
        }
        catch (UserAlreadyExistsException ex)
        {
            throw new RpcException(new Status(StatusCode.AlreadyExists, ex.Message));
        }
        catch (ValidationException ex)
        {
            // Собираем ошибки FluentValidation в одну строку
            var detail = string.Join("; ", ex.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
            throw new RpcException(new Status(StatusCode.InvalidArgument, detail));
        }
        catch (OperationCanceledException)
        {
            throw new RpcException(new Status(StatusCode.Cancelled, "Request was cancelled."));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error in gRPC pipeline");
            throw new RpcException(new Status(StatusCode.Internal, "Unexpected error."));
        }
    }
}