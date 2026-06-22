using System.Text.Json.Serialization;

namespace eSecurity.Core.Security.Authentication.TwoFactor;

public sealed class AuthenticatorTwoFactorPayload : TwoFactorPayload
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}