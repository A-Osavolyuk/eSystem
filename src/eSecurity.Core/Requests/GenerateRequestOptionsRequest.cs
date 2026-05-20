using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests;

public sealed class GenerateRequestOptionsRequest
{
    [JsonPropertyName("subject")]
    public string? Subject { get; set; }
}