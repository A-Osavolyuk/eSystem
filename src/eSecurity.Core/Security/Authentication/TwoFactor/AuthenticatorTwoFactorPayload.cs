namespace eSecurity.Core.Security.Authentication.TwoFactor;

public sealed class AuthenticatorTwoFactorPayload : TwoFactorPayload
{
    public required string Code { get; set; }
}