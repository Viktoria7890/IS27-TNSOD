using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RateLimiter.Writer.Data.Repositories.DbModels;

public sealed class RateLimitDbo
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("route")]
    public string Route { get; set; } = null!;

    [BsonElement("requests_per_minute")]
    public int RequestsPerMinute { get; set; }
}