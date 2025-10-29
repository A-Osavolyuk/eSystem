using eSystem.Auth.Api.Data.Entities;
using eSystem.Core.Attributes;

namespace eSystem.Auth.Api.Security.Authorization.OAuth;

[Injectable(typeof(ILinkedAccountManager), ServiceLifetime.Scoped)]
public class LinkedAccountManager(AuthDbContext context) : ILinkedAccountManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<Result> CreateAsync(UserLinkedAccountEntity linkedAccount, 
        CancellationToken cancellationToken = default)
    {
        await context.UserLinkedAccounts.AddAsync(linkedAccount, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> RemoveAsync(UserLinkedAccountEntity linkedAccount, 
        CancellationToken cancellationToken = default)
    {
        context.UserLinkedAccounts.Remove(linkedAccount);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}