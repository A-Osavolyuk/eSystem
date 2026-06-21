using System.Text.Json.Serialization;

namespace eSecurity.Core.Responses.Verification;

public sealed class ResendEmailOtpResponse
{
    [JsonPropertyName("verification_id")]
    public Guid VerificationId { get; set; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset ExpiresAt { get; set; }
    
    [JsonPropertyName("is_resend_available")]
    public bool IsResendAvailable { get; set; }
    
    [JsonPropertyName("resend_available_at")]
    public DateTimeOffset? ResendAvailableAt { get; set; }
}