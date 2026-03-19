using Grpc.Core;
using UserService.Domain.Services;
using UserService.Grpc.Mappers;
using ProtoU = UserService.Proto;

namespace UserService.Grpc.Services;

public sealed class UsersGrpcService(IUserService service) : ProtoU.UserService.UserServiceBase
{
    public override async Task<ProtoU.UserResponse> CreateUser(ProtoU.CreateUserRequest request,
        ServerCallContext context)
    {
        var created = await service.CreateUserAsync(request.ToDomain(), context.CancellationToken);
        return new ProtoU.UserResponse { User = created.ToProto() };
    }

    public override async Task<ProtoU.UserResponse> GetUserById(ProtoU.GetUserByIdRequest request,
        ServerCallContext context)
    {
        var user = await service.GetUserByIdAsync(request.Id, context.CancellationToken);
        return new ProtoU.UserResponse { User = user.ToProto() };
    }

    public override async Task<ProtoU.UsersResponse> GetUserByName(ProtoU.GetUserByNameRequest request,
        ServerCallContext context)
    {
        var users = await service.GetUserByNameAsync(request.Name, request.Surname, context.CancellationToken);
        return users.ToProtoResponse();
    }

    public override async Task<ProtoU.UserResponse> UpdateUser(ProtoU.UpdateUserRequest request,
        ServerCallContext context)
    {
        var updated = await service.UpdateUserAsync(request.ToDomain(), context.CancellationToken);
        return new ProtoU.UserResponse { User = updated.ToProto() };
    }

    public override async Task<ProtoU.DeleteUserResponse> DeleteUser(ProtoU.DeleteUserRequest request,
        ServerCallContext context)
    {
        await service.DeleteUserAsync(request.Id, context.CancellationToken);
        return new ProtoU.DeleteUserResponse { Deleted = true };
    }
}