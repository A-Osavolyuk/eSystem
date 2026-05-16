using System.Text.Json.Serialization;

namespace eSystem.Core.Server.Security.Authentication.WebFinger;

public sealed class WebFingerResponse
{
    [JsonPropertyName("subject")]
    public required string Subject { get; set; }
    
    [JsonPropertyName("links")]
    public required List<WebFingerLink> Links { get; set; }

    [JsonPropertyName("aliases")]
    public List<string>? Aliases { get; set; }
    
    [JsonPropertyName("properties")]
    public Dictionary<string, string>? Properties { get; set; }
}