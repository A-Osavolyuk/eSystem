using System.Text.Json.Serialization;

namespace eSecurity.Core.Responses.Verification;

public sealed class SendEmailOtpResponse
{
    [JsonPropertyName("verification_id")]
    public required Guid VerificationId { get; init; }

    [JsonPropertyName("expires_at")]
    public required DateTimeOffset ExpiresAt { get; init; }
    
    [JsonPropertyName("max_resend_attempts")]
    public required int MaxResendAttempts { get; init; }
    
    [JsonPropertyName("resend_interval")]
    public required int ResendInterval { get; init; }
}