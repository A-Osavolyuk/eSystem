using System.Text.Json.Serialization;

namespace eSystem.Core.Requests;

public sealed class Metadata
{
    [JsonPropertyName("identifier")]
    public string Identifier { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}