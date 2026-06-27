using System.Text.Json.Serialization;
using eSecurity.WebAuthN.Constants;

namespace eSecurity.WebAuthN;

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
    public Attestation Attestation { get; set; } = Attestation.None;
}

public class ReplyingParty
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("id")]
    public required string Domain { get; set; }
}

public class PublicKeyCredentialUser
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("displayName")]
    public required string DisplayName { get; set; }
}

public class PublicKeyCredentialParameter
{
    [JsonPropertyName("type")]
    public KeyType Type { get; set; }
    
    [JsonPropertyName("alg")]
    public Algorithm Algorithm { get; set; }
}

public class AuthenticatorSelection
{
    [JsonPropertyName("authenticatorAttachment")]
    public AuthenticatorAttachment AuthenticatorAttachment { get; set; } = AuthenticatorAttachment.Platform;

    [JsonPropertyName("residentKey")] 
    public ResidentKey ResidentKey { get; set; } = ResidentKey.Preferred;

    [JsonPropertyName("userVerification")]
    public UserVerification UserVerification { get; set; } = UserVerification.Preferred;
}