using System.Text.Json.Serialization;

namespace eSecurity.Core.Responses.Verification;

public sealed class ResendEmailOtpResponse
{
    [JsonPropertyName("is_resend_available")]
    public bool IsResendAvailable { get; set; }
    
    [JsonPropertyName("resend_available_at")]
    public DateTimeOffset? ResendAvailableAt { get; set; }
}