using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Authentication.Subject.Pairwise;

namespace eSecurity.Idp.Security.Authentication.Subject.Public;

public sealed class PublicSubjectQueryService(AuthDbContext context) : IPublicSubjectQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<PublicSubjectEntity?> GetByUserAsync(Guid userId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.PublicSubjects.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }
}