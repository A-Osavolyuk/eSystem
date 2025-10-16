using System.Text.Json.Serialization;

namespace eShop.Domain.Common.Security.Credentials;

public class PublicKeyCredentialCreationResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    
    [JsonPropertyName("rawId")]
    public string RawId { get; set; } = null!;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = null!;
    
    [JsonPropertyName("response")]
    public CredentialResponse Response { get; set; } = null!;
}

public class CredentialResponse
{
    [JsonPropertyName("attestationObject")]
    public string AttestationObject { get; set; } = null!;
    
    [JsonPropertyName("clientDataJSON")]
    public string ClientDataJson { get; set; } = null!;
    
    [JsonPropertyName("authenticatorData")]
    public string AuthenticatorData { get; set; } = null!;
    
    [JsonPropertyName("publicKey")]
    public string PublicKey { get; set; } = null!;
    
    [JsonPropertyName("publicKeyAlgorithm")]
    public int PublicKeyAlgorithm { get; set; }

    [JsonPropertyName("transports")] 
    public string[] Transports { get; set; } = null!;
}