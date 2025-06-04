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
        var secretEntity = await secretManager.FindAsync(user, cancellationToken);

        if (secretEntity is null)
        {
            var secret = await secretManager.GenerateAsync(user, cancellationToken);
            var qrCode = GenerateQrCode(user.Email, secret, issuer);
            return qrCode;
        }
        else
        {
            var qrCode = GenerateQrCode(user.Email, secretEntity.Secret, issuer);
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