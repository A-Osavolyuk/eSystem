using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Authentication.WebFinger;

public sealed class WebFingerLink
{
    [JsonPropertyName("rel")]
    public required string Rel { get; set; }

    [JsonPropertyName("href")]
    public required string Href { get; set; }
    
    [JsonPropertyName("type")]
    public string? Type { get; set; }
    
    [JsonPropertyName("titles")]
    public Dictionary<string, string>? Titles { get; set; }
    
    [JsonPropertyName("properties")]
    public Dictionary<string, string>? Properties { get; set; }
}