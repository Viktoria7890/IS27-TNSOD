using DomainU = UserService.Domain.Models;
using ProtoU = UserService.Proto;

namespace UserService.Grpc.Mappers;

public static class GrpcDomainMapper
{
    public static DomainU.User ToDomain(this ProtoU.CreateUserRequest request)
    {
        return new DomainU.User(
            0,                 
            request.Login,
            request.Password,
            request.Name,
            request.Surname,
            request.Age
        );
    }

    public static DomainU.User ToDomain(this ProtoU.UpdateUserRequest request)
    {
        return new DomainU.User(
            request.Id,
            "",
            request.Password,
            request.Name,
            request.Surname,
            request.Age
        );
    }

    public static ProtoU.UserDto ToProto(this DomainU.User user)
    {
        var dto = new ProtoU.UserDto
        {
            Id = user.Id,
            Login = user.Login,
            Password = user.Password,
            Name = user.Name,
            Surname = user.Surname,
            Age = user.Age
        };
        return dto;
    }

    public static ProtoU.UsersResponse ToProtoResponse(this DomainU.User[] users)
    {
        var response = new ProtoU.UsersResponse();

        foreach (var user in users) response.Users.Add(user.ToProto());

        return response;
    }
}