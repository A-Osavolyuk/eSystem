using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authorization.Verification;

public sealed class VerificationQueryService(AuthDbContext context) : IVerificationQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<VerificationRequestEntity?> GetByIdAsync(Guid userId, Guid verificationId,
        CancellationToken cancellationToken = default)
    {
        return await _context.VerificationRequests.FirstOrDefaultAsync(
            x => x.UserId == userId && x.Id == verificationId, cancellationToken);
    }
}