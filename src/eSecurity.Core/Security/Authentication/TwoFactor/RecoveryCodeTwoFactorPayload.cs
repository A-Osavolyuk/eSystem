namespace eSecurity.Core.Security.Authentication.TwoFactor;

public sealed class RecoveryCodeTwoFactorPayload : TwoFactorPayload
{
    public required string Code { get; set; }
}