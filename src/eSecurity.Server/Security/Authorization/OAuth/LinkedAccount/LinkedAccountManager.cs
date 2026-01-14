using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;

public class LinkedAccountManager(AuthDbContext context) : ILinkedAccountManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<UserLinkedAccountEntity?> GetAsync(UserEntity user,
        LinkedAccountType type, CancellationToken cancellationToken = default)
    {
        return await _context.UserLinkedAccounts.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Type == type, cancellationToken);
    }

    public async ValueTask<List<UserLinkedAccountEntity>> GetAllAsync(UserEntity user, 
        CancellationToken cancellationToken = default)
    {
        return await _context.UserLinkedAccounts
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(UserLinkedAccountEntity linkedAccount,
        CancellationToken cancellationToken = default)
    {
        await _context.UserLinkedAccounts.AddAsync(linkedAccount, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> RemoveAsync(UserLinkedAccountEntity linkedAccount,
        CancellationToken cancellationToken = default)
    {
        _context.UserLinkedAccounts.Remove(linkedAccount);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<bool> HasAsync(UserEntity user, CancellationToken cancellationToken = default)
        => await _context.UserLinkedAccounts.AnyAsync(x => x.UserId == user.Id, cancellationToken);

    public async ValueTask<bool> HasAsync(UserEntity user, LinkedAccountType type, CancellationToken cancellationToken = default)
        => await _context.UserLinkedAccounts.AnyAsync(
            x => x.UserId == user.Id && x.Type == type, cancellationToken);
}