namespace eSystem.Domain.Responses.Auth;

public class ResendCodeResponse
{
    public int CodeResendAttempts { get; set; }
    public int MaxCodeResendAttempts { get; set; }
    public DateTimeOffset? CodeResendAvailableDate { get; set; }
}