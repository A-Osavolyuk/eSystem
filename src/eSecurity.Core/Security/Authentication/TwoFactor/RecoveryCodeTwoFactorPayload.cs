using System.Text.Json.Serialization;

namespace eSecurity.Core.Security.Authentication.TwoFactor;

public sealed class RecoveryCodeTwoFactorPayload : TwoFactorPayload
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}