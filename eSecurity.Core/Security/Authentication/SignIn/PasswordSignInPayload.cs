namespace eSecurity.Core.Security.Authentication.SignIn;

public sealed class PasswordSignInPayload : SignInPayload
{
    public required string Login { get; set; }
    public required string Password { get; set; }
}