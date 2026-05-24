using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class GenerateRequestOptionsRequest
{
    [JsonPropertyName("user_hint")]
    public string? UserHint { get; set; }
}