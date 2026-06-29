using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authorization.LinkedAccount;

public sealed class LinkedAccountQueryService(AuthDbContext context) : ILinkedAccountQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<UserLinkedAccountEntity>> ListByUserAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserLinkedAccounts
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<UserLinkedAccountEntity?> GetByTypeAsync(Guid userId, LinkedAccountType type,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserLinkedAccounts.FirstOrDefaultAsync(
            x => x.UserId == userId && x.Type == type, cancellationToken);
    }

    public async ValueTask<UserLinkedAccountEntity?> GetByIdAsync(Guid linkedAccountId,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserLinkedAccounts.FirstOrDefaultAsync(
            x => x.Id == linkedAccountId, cancellationToken);
    }

    public async ValueTask<bool> ExistsAsync(Guid userId, LinkedAccountType type,
        CancellationToken cancellationToken = default)
    {
        return await _context.UserLinkedAccounts.AnyAsync(x => x.UserId == userId && x.Type == type, cancellationToken);
    }

    public async ValueTask<bool> HasAnyAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.UserLinkedAccounts.AnyAsync(x => x.UserId == userId, cancellationToken);
    }
}