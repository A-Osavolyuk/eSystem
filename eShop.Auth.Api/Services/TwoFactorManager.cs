using OtpNet;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ITwoFactorManager), ServiceLifetime.Scoped)]
public sealed class TwoFactorManager(
    AuthDbContext context,
    ISecretManager secretManager) : ITwoFactorManager
{
    private readonly AuthDbContext context = context;
    private readonly ISecretManager secretManager = secretManager;
    private const int ExpirationMinutes = 30;

    public async ValueTask<Result> EnableAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        var secret = await secretManager.FindAsync(user, cancellationToken);
        
        if (secret is null)
        {
            await secretManager.GenerateAsync(user, cancellationToken);
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
            throw new Exception("Secret not generated");
        }
        
        var qrCode = GenerateQrCode(user.Email, secret.Secret, issuer);
        return qrCode;
        
        return qrCode;
    }
    
    private string GenerateQrCode(string email, string secret, string issuer)
    {

        var otpUri = new OtpUri(OtpType.Totp, secret, email, issuer);
        var url = otpUri.ToString();
        
        return url;
    }
}