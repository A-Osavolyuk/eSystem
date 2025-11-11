namespace eSecurity.Core.Security.Authentication.SignIn;

public sealed class AuthenticatorSignInPayload : SignInPayload
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
}