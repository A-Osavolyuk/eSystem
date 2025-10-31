namespace eSystem.Core.Security.Authentication.SignIn.Payloads;

public sealed class PasswordSignInPayload : SignInPayload
{
    public required string Login { get; set; }
    public required string Password { get; set; }
}