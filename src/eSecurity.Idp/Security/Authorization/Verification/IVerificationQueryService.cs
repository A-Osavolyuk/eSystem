using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authorization.Verification;

public interface IVerificationQueryService
{
    ValueTask<VerificationRequestEntity?> GetByIdAsync(Guid userId, Guid verificationId,
        CancellationToken cancellationToken = default);
}