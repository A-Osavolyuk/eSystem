namespace eSecurity.Server.Security.Authorization.Verification.AuthenticationApp;

public sealed class AuthenticatorAppVerificationContext : VerificationContext
{
    public required string Code { get; set; }
}