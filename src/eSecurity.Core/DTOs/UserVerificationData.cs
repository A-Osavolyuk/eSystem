using System.Text.Json.Serialization;
using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Core.DTOs;

public class UserVerificationData
{
    [JsonPropertyName("preferred_method")]
    public VerificationMethod PreferredMethod { get; set; }
    
    [JsonPropertyName("email_enabled")]
    public bool EmailEnabled { get; set; }
    
    [JsonPropertyName("authenticator_enabled")]
    public bool AuthenticatorAppEnabled { get; set; }
    
    [JsonPropertyName("passkey_enabled")]
    public bool SoftwareKeyEnabled { get; set; }
}