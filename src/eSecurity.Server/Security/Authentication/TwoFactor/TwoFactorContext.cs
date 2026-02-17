namespace eSecurity.Server.Security.Authentication.TwoFactor;

public abstract class TwoFactorContext
{
    public required Guid TransactionId { get; set; }
}