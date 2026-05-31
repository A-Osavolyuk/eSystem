using System.Text.Json.Serialization;

namespace eSecurity.Core.Responses.Email;

public sealed class ResendEmailVerificationResponse
{
    [JsonPropertyName("is_resend_available")]
    public bool IsResendAvailable { get; set; }
    
    [JsonPropertyName("resend_available_at")]
    public DateTimeOffset? ResendAvailableAt { get; set; }
}