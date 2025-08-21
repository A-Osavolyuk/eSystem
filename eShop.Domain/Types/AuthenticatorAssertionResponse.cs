using System.Text.Json.Serialization;

namespace eShop.Domain.Types;

public class AuthenticatorAssertionResponse
{
    [JsonPropertyName("clientDataJSON")]
    public required string ClientDataJson { get; set; }
    
    [JsonPropertyName("authenticatorData")]
    public required string AuthenticatorData { get; set; }
    
    [JsonPropertyName("signature")]
    public required string Signature { get; set; }
    
    [JsonPropertyName("userHandle")]
    public required string? UserHandle { get; set; }
}