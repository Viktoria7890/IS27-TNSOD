using MongoDB.Driver;
using RateLimiter.Reader.Data.Streams;
using RateLimiter.Reader.Domain.Sevices;
using RateLimiter.Reader.Grpc;
using RateLimiter.Reader.Mappers;
using ReaderService.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

var mongoConn   = builder.Configuration.GetConnectionString("Mongo") 
                  ?? "mongodb://194.26.138.75:27017";
var mongoDbName = builder.Configuration.GetValue<string>("MongoDatabase") 
                  ?? "rate_limiter";

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(mongoConn));
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoDbName);
});

builder.Services.AddSingleton<IRateLimitStreamRepository, RateLimitStreamRepository>();
builder.Services.AddSingleton<IRateLimitReader, RateLimitReaderService>();
builder.Services.AddSingleton<RateLimitMapper>();

builder.Services.AddGrpc();

builder.Services.AddHostedService<ReaderBootstrapper>();

var app = builder.Build();

app.MapGrpcService<ReaderGrpcService>();

await app.RunAsync("http://*:5000");