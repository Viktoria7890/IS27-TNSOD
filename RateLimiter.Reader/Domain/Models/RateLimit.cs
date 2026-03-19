namespace ReaderService.Domain.Models;

public sealed class RateLimit
{
    public string Route { get; set; } = string.Empty;
    public int RequestsPerMinute { get; set; }
}