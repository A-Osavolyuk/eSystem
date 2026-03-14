namespace eSecurity.Server.Security.Authorization.Verification;

public sealed class VerificationConfiguration
{
    public TimeSpan Timestamp { get; set; } = TimeSpan.FromMinutes(10);
}