using System.Text.Json.Serialization;

namespace eSecurity.Core.Responses.Verification;

public sealed class SendEmailOtpResponse
{
    [JsonPropertyName("max_resend_attempts")]
    public int MaxResendAttempts { get; set; }
    
    [JsonPropertyName("resend_interval")]
    public int ResendInterval { get; set; }
}