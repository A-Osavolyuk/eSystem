namespace eSecurity.Core.Security.Authentication.SignIn;

public class TwoFactorSignInPayload : SignInPayload
{
    public Guid UserId { get; set; }
    public required string[] AuthenticationMethods { get; set; }
}