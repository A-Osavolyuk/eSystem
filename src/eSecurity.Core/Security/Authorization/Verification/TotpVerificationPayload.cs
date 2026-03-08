namespace eSecurity.Core.Security.Authorization.Verification;

public sealed class TotpVerificationPayload : VerificationPayload
{
    [JsonPropertyName("code")]
    public required string Code { get; set; }
}