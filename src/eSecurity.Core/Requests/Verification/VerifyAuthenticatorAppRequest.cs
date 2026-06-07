using System.Text.Json.Serialization;

namespace eSecurity.Core.Requests.Verification;

public sealed class VerifyAuthenticatorAppRequest : VerificationRequest
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}