namespace eShop.Auth.Api.Services;

[Injectable(typeof(IPasskeyManager), ServiceLifetime.Scoped)]
public class PasskeyManager(AuthDbContext context) : IPasskeyManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<UserPasskeyEntity?> FindByCredentialIdAsync(string credentialId, CancellationToken cancellationToken)
    {
        var entity = await context.UserPasskeys
            .FirstOrDefaultAsync(x => x.CredentialId == credentialId, cancellationToken);
        
        return entity;
    }
    
    public async ValueTask<UserPasskeyEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.UserPasskeys
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        
        return entity;
    }

    public async ValueTask<Result> CreateAsync(UserPasskeyEntity entity, CancellationToken cancellationToken = default)
    {
        await context.UserPasskeys.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(UserPasskeyEntity entity, CancellationToken cancellationToken = default)
    {
        context.UserPasskeys.Update(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> DeleteAsync(UserPasskeyEntity entity, CancellationToken cancellationToken = default)
    {
        context.UserPasskeys.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}