namespace eShop.Auth.Api.Options;

public class SignInOptions
{
    public bool RequireConfirmedEmail { get; set; } = true;
    public bool RequireConfirmedRecoveryEmail { get; set; } = false;
    public bool RequireConfirmedPhoneNumber { get; set; } = false;
    public bool RequireConfirmedAccount { get; set; } = true;
    
    public bool AllowUserNameLogin { get; set; } = true;
    public bool AllowEmailLogin { get; set; } = true;
    public int MaxFailedLoginAttempts { get; set; } = 5;
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
}