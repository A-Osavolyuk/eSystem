using System.Text.Json.Serialization;

namespace eSystem.Core.Http.Results;

public class Error
{
    [JsonPropertyName("error")]
    public required string Code { get; init; }
    
    [JsonPropertyName("error_description")]
    public required string Description { get; init; }
    
    [JsonPropertyName("error_uri")] 
    public string? Uri { get; set; }

    [JsonPropertyName("details")]
    public Dictionary<string, object> Details { get; set; } = [];
}