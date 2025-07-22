using eShop.Auth.Api.Security.Protection;
using OtpNet;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ITwoFactorManager), ServiceLifetime.Scoped)]
public sealed class TwoFactorManager(
    AuthDbContext context,
    SecretProtector protector,
    ISecretManager secretManager) : ITwoFactorManager
{
    private readonly AuthDbContext context = context;
    private readonly SecretProtector protector = protector;
    private readonly ISecretManager secretManager = secretManager;
    private const int ExpirationMinutes = 30;

    public async ValueTask<Result> EnableAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        if (!await context.Users.AnyAsync(u => u.Email == user.Email, cancellationToken))
        {
            var providers = await context.Providers
                .Select(p => new UserProviderEntity()
                {
                    UserId = user.Id, 
                    ProviderId = p.Id, 
                    Subscribed = false, 
                    CreateDate = DateTimeOffset.UtcNow
                })
                .ToListAsync(cancellationToken);
            
            await context.UserProvider.AddRangeAsync(providers, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        
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

    public async ValueTask<string> GenerateQrCodeAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        const string issuer = "eShop";
        var secret = await secretManager.FindAsync(user, cancellationToken);

        if (secret is null)
        {
            secret = await secretManager.GenerateAsync(user, cancellationToken);
            var unprotectedSecret = protector.Unprotect(secret.Secret);
            var qrCode = GenerateQrCode(user.Email, unprotectedSecret, issuer);
            return qrCode;
        }
        else
        {
            var unprotectedSecret = protector.Unprotect(secret.Secret);
            var qrCode = GenerateQrCode(user.Email, unprotectedSecret, issuer);
            return qrCode;
        }
    }
    
    private string GenerateQrCode(string email, string secret, string issuer)
    {

        var otpUri = new OtpUri(OtpType.Totp, secret, email, issuer);
        var url = otpUri.ToString();
        
        return url;
    }
}