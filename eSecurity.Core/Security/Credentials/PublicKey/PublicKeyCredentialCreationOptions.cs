using System.Text.Json.Serialization;
using eSecurity.Core.Security.Credentials.PublicKey.Constants;

namespace eSecurity.Core.Security.Credentials.PublicKey;

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
    public required string Type { get; set; }
    
    [JsonPropertyName("alg")]
    public int Algorithm { get; set; }
}

public class AuthenticatorSelection
{
    [JsonPropertyName("authenticatorAttachment")]
    public string AuthenticatorAttachment { get; set; } = AuthenticatorAttachments.Platform;

    [JsonPropertyName("residentKey")] 
    public string ResidentKey { get; set; } = ResidentKeys.Preferred;

    [JsonPropertyName("userVerification")]
    public string UserVerification { get; set; } = UserVerifications.Preferred;
}