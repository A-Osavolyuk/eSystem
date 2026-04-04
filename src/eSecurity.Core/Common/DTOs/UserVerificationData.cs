using eSecurity.Core.Security.Authorization.Verification;
using eSystem.Core.Enums;

namespace eSecurity.Core.Common.DTOs;

public class UserVerificationData
{
    [JsonPropertyName("preferred_method")]
    [JsonConverter(typeof(JsonEnumValueStringConverter<VerificationMethod>))]
    public VerificationMethod PreferredMethod { get; set; }
    
    [JsonPropertyName("email_enabled")]
    public bool EmailEnabled { get; set; }
    
    [JsonPropertyName("authenticator_enabled")]
    public bool AuthenticatorEnabled { get; set; }
    
    [JsonPropertyName("passkey_enabled")]
    public bool PasskeyEnabled { get; set; }
}