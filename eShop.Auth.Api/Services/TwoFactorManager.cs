using eShop.Auth.Api.Security.Protection;
using OtpNet;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ITwoFactorManager), ServiceLifetime.Scoped)]
public sealed class TwoFactorManager(
    AuthDbContext context,
    SecretProtector protector,
    ISecretManager secretManager,
    IUserManager userManager) : ITwoFactorManager
{
    private readonly AuthDbContext context = context;
    private readonly SecretProtector protector = protector;
    private readonly ISecretManager secretManager = secretManager;
    private readonly IUserManager userManager = userManager;
    private const int ExpirationMinutes = 30;

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