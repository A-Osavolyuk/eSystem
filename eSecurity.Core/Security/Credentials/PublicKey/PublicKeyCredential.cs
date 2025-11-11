using System.Text.Json.Serialization;

namespace eSecurity.Core.Security.Credentials.PublicKey;

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