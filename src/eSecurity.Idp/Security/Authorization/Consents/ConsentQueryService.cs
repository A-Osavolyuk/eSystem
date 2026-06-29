using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authorization.Consents;

public sealed class ConsentQueryService(AuthDbContext context) : IConsentQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<ConsentEntity?> GetByClientAsync(Guid userId, Guid clientId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Consents.FirstOrDefaultAsync(
            x => x.UserId == userId && x.ClientId == clientId, cancellationToken);
    }

    public async ValueTask<ConsentEntity?> GetByIdAsync(Guid consentId, CancellationToken cancellationToken = default)
    {
        return await _context.Consents.FirstOrDefaultAsync(x => x.Id == consentId, cancellationToken);
    }
}