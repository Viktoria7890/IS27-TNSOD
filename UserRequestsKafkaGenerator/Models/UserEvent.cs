using System.Text.Json.Serialization;

namespace UserRequestsKafkaGenerator.Models;

public sealed class UserEvent
{
    [JsonPropertyName("user_id")]
    public int UserId { get; set; }

    [JsonPropertyName("endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    public override string ToString() =>
        $"UserEvent(user_id={UserId}, endpoint={Endpoint})";
}