namespace eShop.Auth.Api.Security.Identity.Options;

public class CodeOptions
{
    public int MaxCodeResendAttempts { get; set; } = 5;
    public int CodeResendUnavailableTime { get; set; } = 2;
}