using System.Text.Json.Serialization;

namespace eShop.Domain.Types;

public class PublicKeyCredential
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("rawId")]
    public required string RawId { get; set; }
    
    [JsonPropertyName("type")]
    public required string Type { get; set; }
    
    [JsonPropertyName("response")]
    public required AuthenticatorAssertionResponse Response { get; set; }
}