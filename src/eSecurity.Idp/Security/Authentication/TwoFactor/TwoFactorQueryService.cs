using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public sealed class TwoFactorQueryService(AuthDbContext context) : ITwoFactorQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<UserTwoFactorMethodEntity>> ListByUserAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserTwoFactorMethods
            .Where(x => x.UserId == userId)
            .Include(x => x.Method)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<UserTwoFactorMethodEntity?> GetByMethodAsync(Guid userId, TwoFactorMethod method,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserTwoFactorMethods
            .Include(x => x.Method)
            .FirstOrDefaultAsync(x => x.UserId == userId && x.Method.Type == method, cancellationToken);
    }
}