using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Identity.Privacy;

public sealed class PersonalDataQueryService(AuthDbContext context) : IPersonalDataQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<PersonalDataEntity?> GetByUserAsync(Guid userId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.PersonalData.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }
}