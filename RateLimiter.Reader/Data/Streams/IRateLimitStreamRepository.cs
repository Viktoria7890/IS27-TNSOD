using MongoDB.Driver;
using RateLimiter.Reader.Data.DbModels;

namespace RateLimiter.Reader.Data.Streams;

public interface IRateLimitStreamRepository
{
    IAsyncEnumerable<IReadOnlyList<RateLimitDbModel>> LoadAllInBatchesAsync(int batchSize = 1000);
    IAsyncEnumerable<ChangeStreamDocument<RateLimitDbModel>> WatchChangesAsync();
    Task EnsureIndexesAsync();
}