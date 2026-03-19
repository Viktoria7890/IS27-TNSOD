using System.Runtime.CompilerServices;
using MongoDB.Driver;
using RateLimiter.Reader.Data.DbModels;

namespace RateLimiter.Reader.Data.Streams;

public sealed class RateLimitStreamRepository : IRateLimitStreamRepository
{
    private readonly IMongoCollection<RateLimitDbModel> _col;

    public RateLimitStreamRepository(IMongoDatabase db)
        => _col = db.GetCollection<RateLimitDbModel>("rate_limits");

    public IAsyncEnumerable<IReadOnlyList<RateLimitDbModel>> LoadAllInBatchesAsync(int batchSize = 1000)
        => LoadAllInBatchesInternalAsync(batchSize, CancellationToken.None);

    public IAsyncEnumerable<ChangeStreamDocument<RateLimitDbModel>> WatchChangesAsync()
        => WatchChangesInternalAsync(CancellationToken.None);

    public async Task EnsureIndexesAsync()
    {
        var keys  = Builders<RateLimitDbModel>.IndexKeys.Ascending(x => x.Route);
        var model = new CreateIndexModel<RateLimitDbModel>(
            keys,
            new CreateIndexOptions { Unique = true, Name = "ux_route" }
        );
        await _col.Indexes.CreateOneAsync(model);
    }

    private async IAsyncEnumerable<IReadOnlyList<RateLimitDbModel>> LoadAllInBatchesInternalAsync(
        int batchSize,
        [EnumeratorCancellation] CancellationToken ct)
    {
        if (batchSize <= 0) batchSize = 1000;

        using var cursor = await _col.FindAsync(FilterDefinition<RateLimitDbModel>.Empty, cancellationToken: ct);
        var buffer = new List<RateLimitDbModel>(batchSize);

        while (await cursor.MoveNextAsync(ct))
        {
            foreach (var doc in cursor.Current)
            {
                buffer.Add(doc);
                if (buffer.Count >= batchSize)
                {
                    yield return buffer.ToArray();
                    buffer.Clear();
                }
            }
        }

        if (buffer.Count > 0)
            yield return buffer.ToArray();
    }

    private async IAsyncEnumerable<ChangeStreamDocument<RateLimitDbModel>> WatchChangesInternalAsync(
        [EnumeratorCancellation] CancellationToken ct)
    {
        var opts = new ChangeStreamOptions
        {
            FullDocument = ChangeStreamFullDocumentOption.UpdateLookup
        };

        var pipeline = new EmptyPipelineDefinition<ChangeStreamDocument<RateLimitDbModel>>()
            .Match(x => x.OperationType == ChangeStreamOperationType.Insert
                     || x.OperationType == ChangeStreamOperationType.Update
                     || x.OperationType == ChangeStreamOperationType.Replace
                     || x.OperationType == ChangeStreamOperationType.Delete);

        using var cursor = await _col.WatchAsync(pipeline, opts, ct);

        while (await cursor.MoveNextAsync(ct))
        {
            foreach (var change in cursor.Current)
                yield return change;
        }
    }
}
