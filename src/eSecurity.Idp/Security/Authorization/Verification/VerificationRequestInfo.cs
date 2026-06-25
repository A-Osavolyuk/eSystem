using eSecurity.Core.Security.Authorization.Verification;

namespace eSecurity.Idp.Security.Authorization.Verification;

public sealed class VerificationRequestInfo(VerificationStatus status, DateTimeOffset expiredAt)
{
    public VerificationStatus Status { get; init; } = status;
    public DateTimeOffset ExpiredAt { get; init; } = expiredAt;
}