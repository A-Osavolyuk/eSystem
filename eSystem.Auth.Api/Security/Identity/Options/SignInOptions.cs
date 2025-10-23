namespace eSystem.Auth.Api.Security.Identity.Options;

public class SignInOptions
{
    public bool RequireConfirmedEmail { get; set; } = true;
    public bool RequireConfirmedRecoveryEmail { get; set; } = false;
    public bool RequireConfirmedPhoneNumber { get; set; } = false;
    public bool RequireConfirmedAccount { get; set; } = true;
    public bool RequireTrustedDevice { get; set; } = true;
    
    public bool AllowUserNameLogin { get; set; } = true;
    public bool AllowEmailLogin { get; set; } = true;
    public bool AllowOAuthLogin { get; set; } = true;
    public int MaxFailedLoginAttempts { get; set; } = 5;
}