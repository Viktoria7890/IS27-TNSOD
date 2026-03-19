using Microsoft.AspNetCore.Server.Kestrel.Core;
using Npgsql;
using UserService.Data.Repositories;
using UserService.Domain.Services;
using UserService.Domain.Validators;
using UserService.Grpc.Interceptors;
using UserService.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5002, listenOptions => listenOptions.Protocols = HttpProtocols.Http2);
});

builder.Services.AddSingleton<CreateUserValidator>();
builder.Services.AddSingleton<UpdateUserValidator>();

var connString = builder.Configuration.GetConnectionString("Main")!;
var dataSourceBuilder = new NpgsqlDataSourceBuilder(connString);

var dataSource = dataSourceBuilder.Build();

builder.Services.AddSingleton(dataSource);

builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IUserService, UserService.Domain.Services.UserService>();

var app = builder.Build();

builder.Services.AddGrpc(o => { o.Interceptors.Add<GlobalExceptionInterceptor>(); });
app.MapGrpcService<UsersGrpcService>();

await app.RunAsync();