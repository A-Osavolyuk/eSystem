namespace eShop.Auth.Api.Services;

[Injectable(typeof(ICredentialManager), ServiceLifetime.Scoped)]
public class CredentialManager(AuthDbContext context) : ICredentialManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<Result> CreateAsync(UserCredentialEntity entity, CancellationToken cancellationToken = default)
    {
        await context.UserCredentials.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}