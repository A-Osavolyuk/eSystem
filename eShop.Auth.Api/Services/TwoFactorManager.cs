namespace eShop.Auth.Api.Services;

public class TwoFactorManager(AuthDbContext context) : ITwoFactorManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<List<ProviderEntity>> GetProvidersAsync(CancellationToken cancellationToken = default)
    {
        var providers = await context.Providers.ToListAsync(cancellationToken);
        return providers;
    }

    public async ValueTask<Result> EnableAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        user.TwoFactorEnabled = true;
        user.UpdateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> DisableAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        user.TwoFactorEnabled = false;
        user.UpdateDate = DateTime.UtcNow;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    
    public async ValueTask<string> GenerateTokenAsync(UserEntity user, Provider provider,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<string> GenerateQrAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async ValueTask<bool> VerifyTokenAsync(UserEntity user, Provider provider, string token,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}