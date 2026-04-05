using System.Text.Json.Serialization;

namespace eSystem.Core.Primitives;

public class Error
{
    [JsonPropertyName("error")]
    public required ErrorCode Code { get; init; }
    
    [JsonPropertyName("error_description")]
    public required string Description { get; init; }
    
    [JsonPropertyName("error_uri")] 
    public string? Uri { get; set; }

    [JsonPropertyName("details")]
    public Dictionary<string, object> Details { get; set; } = [];
}