namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILinkedAccountManager), ServiceLifetime.Scoped)]
public class LinkedAccountManager(AuthDbContext context) : ILinkedAccountManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<Result> CreateAsync(UserLinkedAccountEntity linkedAccount, 
        CancellationToken cancellationToken = default)
    {
        await context.UserOAuthProviders.AddAsync(linkedAccount, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> RemoveAsync(UserLinkedAccountEntity linkedAccount, 
        CancellationToken cancellationToken = default)
    {
        context.UserOAuthProviders.Remove(linkedAccount);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> AllowAsync(UserLinkedAccountEntity linkedAccount, 
        CancellationToken cancellationToken = default)
    {
        linkedAccount.Allowed = true;
        linkedAccount.UpdateDate = DateTimeOffset.UtcNow;
        
        context.UserOAuthProviders.Update(linkedAccount);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> DisallowAsync(UserLinkedAccountEntity linkedAccount, 
        CancellationToken cancellationToken = default)
    {
        linkedAccount.Allowed = false;
        linkedAccount.UpdateDate = DateTimeOffset.UtcNow;
        
        context.UserOAuthProviders.Update(linkedAccount);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}