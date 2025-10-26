using System.Text.Json.Serialization;

namespace eSystem.Core.Security.Credentials.PublicKey;

public class PublicKeyCredentialCreationResponse
{
    [JsonPropertyName("id")]
    public required string Id { get; set; } = null!;
    
    [JsonPropertyName("rawId")]
    public required string RawId { get; set; } = null!;
    
    [JsonPropertyName("type")]
    public required string Type { get; set; } = null!;
    
    [JsonPropertyName("response")]
    public required CredentialResponse Response { get; set; } = null!;
}

public class CredentialResponse
{
    [JsonPropertyName("attestationObject")]
    public required string AttestationObject { get; set; } = null!;
    
    [JsonPropertyName("clientDataJSON")]
    public required string ClientDataJson { get; set; } = null!;
    
    [JsonPropertyName("authenticatorData")]
    public required string AuthenticatorData { get; set; } = null!;
    
    [JsonPropertyName("publicKey")]
    public required string PublicKey { get; set; } = null!;
    
    [JsonPropertyName("publicKeyAlgorithm")]
    public required int PublicKeyAlgorithm { get; set; }

    [JsonPropertyName("transports")] 
    public required string[] Transports { get; set; } = null!;
}