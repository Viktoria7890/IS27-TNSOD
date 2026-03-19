using System.Collections.Concurrent;
using MongoDB.Driver;
using RateLimiter.Reader.Data.DbModels;
using RateLimiter.Reader.Data.Streams;
using RateLimiter.Reader.Domain.Sevices;
using ReaderService.Domain.Models;

namespace ReaderService.Domain.Services;

public sealed class RateLimitReaderService : IRateLimitReader
{
    private readonly IRateLimitStreamRepository _repo;
    private readonly ConcurrentDictionary<string,int> _limits = new();
    private Task? _watchTask;

    public RateLimitReaderService(IRateLimitStreamRepository repo)
    {
        _repo = repo;
    }

    public async Task StartAsync()
    {
        await foreach (var batch in _repo.LoadAllInBatchesAsync(1000))
        {
            foreach (var db in batch)
            {
                _limits[db.Route] = db.RequestsPerMinute;
            }
        }
        
        _watchTask = Task.Run((Func<Task>)(async () =>
        {
            await foreach (var change in _repo.WatchChangesAsync())
            {
                ApplyChange(change);
            }
        }));
    }

    public IReadOnlyList<RateLimit> GetAll()
        => _limits.Select(kv => new RateLimit { Route = kv.Key, RequestsPerMinute = kv.Value })
                  .OrderBy(x => x.Route)
                  .ToList();

    private void ApplyChange(ChangeStreamDocument<RateLimitDbModel> change)
    {
        switch (change.OperationType)
        {
            case ChangeStreamOperationType.Insert:
            case ChangeStreamOperationType.Replace:
            case ChangeStreamOperationType.Update:
                if (change.FullDocument is { } doc)
                    _limits[doc.Route] = doc.RequestsPerMinute;
                break;

            case ChangeStreamOperationType.Delete:
                if (change.DocumentKey != null && change.DocumentKey.TryGetValue("route", out var routeToken))
                {
                    var route = routeToken.AsString;
                    _limits.TryRemove(route, out _);
                }
                else
                {
                }
                break;
        }
    }
}
