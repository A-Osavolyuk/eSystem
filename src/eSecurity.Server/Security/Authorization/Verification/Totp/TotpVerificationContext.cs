namespace eSecurity.Server.Security.Authorization.Verification.Totp;

public sealed class TotpVerificationContext : VerificationContext
{
    public required string Code { get; set; }
}