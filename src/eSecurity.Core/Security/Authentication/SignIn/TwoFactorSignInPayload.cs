namespace eSecurity.Core.Security.Authentication.SignIn;

public class TwoFactorSignInPayload : SignInPayload
{
    public Guid TransactionId { get; set; }
    public required string AuthenticationMethod { get; set; }
}