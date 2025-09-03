using System.Text.Json.Serialization;
using eShop.Domain.Common.Security.Credentials;

namespace eShop.Domain.Types;

public class PublicKeyCredentialCreationOptions
{
    [JsonPropertyName("challenge")]
    public required string Challenge { get; set; }
    
    [JsonPropertyName("rp")]
    public required ReplyingParty ReplyingParty { get; set; }
    
    [JsonPropertyName("user")] 
    public required PublicKeyCredentialUser PublicKeyCredentialUser { get; set; }
    
    [JsonPropertyName("pubKeyCredParams")]
    public required List<PublicKeyCredentialParameter> PublicKeyCredentialParameters { get; set; }

    [JsonPropertyName("authenticatorSelection")]
    public required AuthenticatorSelection AuthenticatorSelection { get; set; }

    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = 60000;

    [JsonPropertyName("attestation")] 
    public string Attestation { get; set; } = Attestations.None;
}