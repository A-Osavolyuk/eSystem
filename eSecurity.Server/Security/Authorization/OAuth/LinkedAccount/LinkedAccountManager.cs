using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;

public class LinkedAccountManager(AuthDbContext context) : ILinkedAccountManager
{
    private readonly AuthDbContext _context = context;

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
}