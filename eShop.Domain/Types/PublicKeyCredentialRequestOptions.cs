using System.Text.Json.Serialization;
using eShop.Domain.Common.Security.Credentials;

namespace eShop.Domain.Types;

public class PublicKeyCredentialRequestOptions
{
    [JsonPropertyName("challenge")]
    public required string Challenge { get; set; }
    
    [JsonPropertyName("timeout")]
    public required int Timeout { get; set; }
    
    [JsonPropertyName("rpId")]
    public required string Domain { get; set; }

    [JsonPropertyName("userVerification")]
    public required string UserVerification { get; set; } = UserVerifications.Preferred;
    
    [JsonPropertyName("allowedCredentials")]
    public required List<AllowedCredential> AllowedCredentials { get; set; }
}