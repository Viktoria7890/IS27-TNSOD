namespace UserRequestsKafkaGenerator.Jobs;

public sealed class UserEventJob
{
    public Guid JobId { get; init; } = Guid.NewGuid();

    public int UserId { get; set; }
    public string Endpoint { get; set; } = string.Empty;
    public int Rpm { get; set; }

    public TimeSpan Interval => TimeSpan.FromMinutes(1.0 / Rpm);
    public DateTime NextDueUtc { get; set; } = DateTime.UtcNow;

    public override string ToString() =>
        $"JobId={JobId}, user_id={UserId}, endpoint={Endpoint}, rpm={Rpm}";
}
