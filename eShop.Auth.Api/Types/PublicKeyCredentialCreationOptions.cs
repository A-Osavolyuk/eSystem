using System.Text.Json.Serialization;

namespace eShop.Auth.Api.Types;

public class PublicKeyCredentialCreationOptions
{
    [JsonPropertyName("challenge")]
    public required byte[] Challenge { get; set; }
    
    [JsonPropertyName("rp")]
    public required ReplyingParty ReplyingParty { get; set; }
    
    [JsonPropertyName("user")] 
    public required User User { get; set; }
    
    [JsonPropertyName("pubKeyCredParams")]
    public required List<PublicKeyCredentialParameter> PublicKeyCredentialParameters { get; set; }

    [JsonPropertyName("authenticatorSelection")]
    public required AuthenticatorSelection AuthenticatorSelection { get; set; }

    [JsonPropertyName("timeout")]
    public int Timeout { get; set; } = 60000;

    [JsonPropertyName("attestation")] 
    public Attestation Attestation { get; set; } = Attestation.None;
}