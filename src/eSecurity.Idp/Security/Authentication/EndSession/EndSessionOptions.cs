namespace eSecurity.Idp.Security.Authentication.EndSession;

public sealed class EndSessionOptions
{
    public string? FallbackUrl { get; set; }
    public string? LogoutUrl { get; set; }
    public string? LoggedOutUrl { get; set; }
    public TimeSpan Timestamp { get; set; }
    public TimeSpan? FrontchannelRedirectDelay { get; set; }
}