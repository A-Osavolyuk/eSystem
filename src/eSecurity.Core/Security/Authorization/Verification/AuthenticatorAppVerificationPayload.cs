namespace eSecurity.Core.Security.Authorization.Verification;

public sealed class AuthenticatorAppVerificationPayload : VerificationPayload
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}