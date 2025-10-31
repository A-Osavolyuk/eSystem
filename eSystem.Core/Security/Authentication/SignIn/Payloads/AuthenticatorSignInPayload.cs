namespace eSystem.Core.Security.Authentication.SignIn.Payloads;

public sealed class AuthenticatorSignInPayload : SignInPayload
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
}