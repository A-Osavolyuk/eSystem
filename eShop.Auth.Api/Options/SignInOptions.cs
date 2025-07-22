namespace eShop.Auth.Api.Options;

public class SignInOptions
{
    public bool AllowUserNameLogin { get; set; } = false;
    public bool AllowEmailLogin { get; set; } = true;
    public int MaxFailedAccessAttempts { get; set; } = 5;
    public TimeSpan LockoutDuration { get; set; } = TimeSpan.FromMinutes(15);
}