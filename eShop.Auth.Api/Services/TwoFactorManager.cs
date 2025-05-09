using eShop.Domain.Common.Security;
using OtpNet;

namespace eShop.Auth.Api.Services;

public class TwoFactorManager(AuthDbContext context) : ITwoFactorManager
{
    private readonly AuthDbContext context = context;
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

    public async ValueTask<string> GenerateTokenAsync(UserEntity user, ProviderEntity provider,
        CancellationToken cancellationToken = default)
    {
        var existingToken = await FindAsync(user, provider, cancellationToken);

        if (existingToken is not null)
        {
            if (existingToken.ExpireDate < DateTime.UtcNow)
            {
                return existingToken.Token;
            }

            await DeleteAsync(existingToken, cancellationToken);
        }

        var token = SecurityHandler.GenerateToken();

        var entity = new LoginTokenEntity()
        {
            Id = Guid.CreateVersion7(),
            ProviderId = provider.Id,
            UserId = user.Id,
            Token = token,
            ExpireDate = DateTime.UtcNow.AddMinutes(ExpirationMinutes),
            CreateDate = DateTime.UtcNow,
            UpdateDate = null,
        };

        await context.LoginTokens.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return token;
    }

    public async ValueTask<Result> VerifyTokenAsync(UserEntity user, ProviderEntity provider, string token,
        CancellationToken cancellationToken = default)
    {
        if (provider.Name == Providers.Authenticator)
        {
            var userSecret = await context.UserSecret.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

            if (userSecret is null)
            {
                return Results.NotFound("Not found user secret");
            }

            var isTokenVerified = SecurityHandler.VerifyAuthenticatorToken(userSecret.Secret, token);

            return !isTokenVerified ? Results.BadRequest("Invalid token") : Result.Success();
        }

        var entity = await context.LoginTokens
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Token == token, cancellationToken);

        if (entity is null)
        {
            return Results.NotFound("Not found token");
        }

        if (entity.ExpireDate < DateTime.UtcNow)
        {
            return Results.BadRequest("Token expired");
        }

        await DeleteAsync(entity, cancellationToken);

        return Result.Success();
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
            var secret = SecurityHandler.GenerateSecret();

            var userSecret = new UserSecretEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                Secret = secret,
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            };

            await context.UserSecret.AddAsync(userSecret, cancellationToken);
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
            var userSecret = await context.UserSecret.FirstAsync(x => x.UserId == user.Id, cancellationToken);
            context.UserSecret.Remove(userSecret);
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

    private async ValueTask DeleteAsync(LoginTokenEntity entity, CancellationToken cancellationToken = default)
    {
        context.LoginTokens.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }
}