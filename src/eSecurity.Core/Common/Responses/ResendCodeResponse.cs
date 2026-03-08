namespace eSecurity.Core.Common.Responses;

public class ResendCodeResponse
{
    [JsonPropertyName("code_resend_attempts")]
    public int CodeResendAttempts { get; set; }
    
    [JsonPropertyName("max_code_resend_attempts")]
    public int MaxCodeResendAttempts { get; set; }
    
    [JsonPropertyName("code_resend_available_date")]
    public DateTimeOffset? CodeResendAvailableDate { get; set; }
}