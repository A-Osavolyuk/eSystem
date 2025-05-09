using eShop.Domain.Common.Security;
using OtpNet;

namespace eShop.Auth.Api.Services;

public class TwoFactorManager(AuthDbContext context) : ITwoFactorManager
{
    private readonly AuthDbContext context = context;
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

    public async ValueTask<QrCode> GenerateQrCodeAsync(UserEntity user, string secret,
        CancellationToken cancellationToken = default)
    {
        const string issuer = "eShop";
        var qrCode = SecurityHandler.GenerateQrCode(user.Email, secret, issuer);

        return await Task.FromResult(qrCode);
    }
}