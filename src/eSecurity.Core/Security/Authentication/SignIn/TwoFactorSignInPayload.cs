namespace eSecurity.Core.Security.Authentication.SignIn;

public class TwoFactorSignInPayload : SignInPayload
{
    public Guid Sid { get; set; }
}