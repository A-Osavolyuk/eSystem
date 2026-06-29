using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.Subject.Pairwise;

public sealed class PairwiseSubjectQueryService(AuthDbContext context) : IPairwiseSubjectQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<PairwiseSubjectEntity?> GetByClientAsync(Guid userId, Guid clientId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.PairwiseSubjects.FirstOrDefaultAsync(
            x => x.UserId == userId && x.ClientId == clientId, cancellationToken);
    }
}