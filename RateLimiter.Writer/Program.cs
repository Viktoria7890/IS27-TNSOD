using MongoDB.Driver;
using RateLimiter.Domain.Validators;
using RateLimiter.Writer.Data.Repositories;
using RateLimiter.Writer.Domain.Services;
using RateLimiter.Writer.Grpc;
using RateLimiter.Writer.Mappers;

var builder = WebApplication.CreateBuilder(args);

var mongoConn   = builder.Configuration.GetConnectionString("Mongo") ?? "mongodb://localhost:27017";
var mongoDbName = builder.Configuration.GetValue<string>("MongoDatabase") ?? "rate_limiter";

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDbName);
});

builder.Services.AddScoped<IRateLimitRepository, RateLimitRepository>();
builder.Services.AddScoped<IRateLimitService, RateLimitService>();
builder.Services.AddSingleton<CreateRateLimitValidator>();
builder.Services.AddSingleton<UpdateRateLimitValidator>();
builder.Services.AddSingleton<RateLimitMapper>();
builder.Services.AddSingleton<ExceptionMappingInterceptor>();

builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ExceptionMappingInterceptor>();
});

builder.Services.AddGrpc();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var repo = scope.ServiceProvider.GetRequiredService<IRateLimitRepository>();
    await repo.EnsureIndexesAsync(CancellationToken.None);
}

app.MapGrpcService<WriterGrpcService>();
app.MapGet("/", () => "RateLimiter.Writer is running");

await app.RunAsync("http://*:5001");