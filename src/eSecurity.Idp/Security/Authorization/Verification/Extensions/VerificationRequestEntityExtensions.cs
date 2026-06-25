using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authorization.Verification.Extensions;

public static class VerificationRequestEntityExtensions
{
    public static VerificationRequestInfo ToInfo(this VerificationRequestEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new VerificationRequestInfo(entity.Status, entity.ExpiredAt);
    }
}