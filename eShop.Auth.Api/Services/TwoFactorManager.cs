using eShop.Domain.Common.Security;
using OtpNet;

namespace eShop.Auth.Api.Services;

public class TwoFactorManager(
    AuthDbContext context,
    ISecretManager secretManager) : ITwoFactorManager
{
    private readonly AuthDbContext context = context;
    private readonly ISecretManager secretManager = secretManager;
    private const int ExpirationMinutes = 30;

    public async ValueTask<List<ProviderEntity>> GetProvidersAsync(CancellationToken cancellationToken = default)
    {
        var providers = await context.Providers.ToListAsync(cancellationToken);
        return providers;
    }

    public async ValueTask<List<ProviderEntity>> GetProvidersAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        var providers = await context.UserProvider
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Provider)
            .Select(x => x.Provider)
            .ToListAsync(cancellationToken);

        return providers;
    }

    public async ValueTask<ProviderEntity?> GetProviderAsync(string providerName,
        CancellationToken cancellationToken = default)
    {
        var provider = await context.Providers.FirstOrDefaultAsync(x => x.Name == providerName, cancellationToken);
        return provider;
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

    public async ValueTask<QrCode> GenerateQrCodeAsync(UserEntity user, string secret,
        CancellationToken cancellationToken = default)
    {
        const string issuer = "eShop";
        var qrCode = SecurityHandler.GenerateQrCode(user.Email, secret, issuer);

        return await Task.FromResult(qrCode);
    }

    public async ValueTask<Result> SubscribeAsync(UserEntity user, ProviderEntity provider,
        CancellationToken cancellationToken = default)
    {
        var userProvider = new UserProviderEntity()
        {
            UserId = user.Id,
            ProviderId = provider.Id,
            CreateDate = DateTime.UtcNow,
            UpdateDate = null
        };

        if (provider.Name == Providers.Authenticator)
        {
            await secretManager.GenerateAsync(user, cancellationToken);
        }

        await context.UserProvider.AddAsync(userProvider, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnsubscribeAsync(UserEntity user, ProviderEntity provider,
        CancellationToken cancellationToken = default)
    {
        var userProvider = await context.UserProvider
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.ProviderId == provider.Id, cancellationToken);

        if (userProvider is null)
        {
            return Results.NotFound("Not found user provider");
        }

        if (provider.Name == Providers.Authenticator)
        {
            await secretManager.DeleteAsync(user, cancellationToken);
        }

        context.UserProvider.Remove(userProvider);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async ValueTask<LoginTokenEntity?> FindAsync(UserEntity user, ProviderEntity provider,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.LoginTokens
            .Where(x => x.UserId == user.Id && x.ProviderId == provider.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return entity;
    }
}