namespace eSecurity.Core.Security.Authentication.SignIn.Session;

public class TwoFactorSignInPayload : SignInPayload
{
    public Guid Sid { get; set; }
}