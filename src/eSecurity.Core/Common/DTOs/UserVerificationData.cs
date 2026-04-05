using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Core.Common.DTOs;

public class UserVerificationData
{
    [JsonPropertyName("preferred_method")]
    public VerificationMethod PreferredMethod { get; set; }
    
    [JsonPropertyName("email_enabled")]
    public bool EmailEnabled { get; set; }
    
    [JsonPropertyName("authenticator_enabled")]
    public bool AuthenticatorEnabled { get; set; }
    
    [JsonPropertyName("passkey_enabled")]
    public bool PasskeyEnabled { get; set; }
}