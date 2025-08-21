namespace eShop.Auth.Api.Services;

[Injectable(typeof(ICredentialManager), ServiceLifetime.Scoped)]
public class CredentialManager(AuthDbContext context) : ICredentialManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<UserCredentialEntity?> FindByCredentialIdAsync(string credentialId, CancellationToken cancellationToken)
    {
        var entity = await context.UserCredentials
            .FirstOrDefaultAsync(x => x.CredentialId == credentialId, cancellationToken);
        
        return entity;
    }
    
    public async ValueTask<UserCredentialEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.UserCredentials
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity;
    }

    public async ValueTask<Result> CreateAsync(UserCredentialEntity entity, CancellationToken cancellationToken = default)
    {
        await context.UserCredentials.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(UserCredentialEntity entity, CancellationToken cancellationToken = default)
    {
        context.UserCredentials.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> DeleteAsync(UserCredentialEntity entity, CancellationToken cancellationToken = default)
    {
        context.UserCredentials.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}