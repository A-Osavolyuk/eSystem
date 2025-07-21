using eShop.Domain.Common.Security;
using OtpNet;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILoginTokenManager), ServiceLifetime.Scoped)]
public sealed class LoginCodeManager(
    AuthDbContext context,
    ISecretManager secretManager) : ILoginTokenManager
{
    private readonly AuthDbContext context = context;
    private readonly ISecretManager secretManager = secretManager;
    private const int ExpirationMinutes = 30;

    public async ValueTask<string> GenerateAsync(UserEntity user, 
        ProviderEntity provider, CancellationToken cancellationToken = default)
    {
        var existingToken = await context.LoginCodes
            .Where(x => x.UserId == user.Id && x.ProviderId == provider.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingToken is not null)
        {
            if (existingToken.ExpireDate < DateTime.UtcNow)
            {
                return existingToken.Code;
            }

            await DeleteAsync(existingToken, cancellationToken);
        }

        var rnd = new Random();
        var code = rnd.Next(0, 999999).ToString().PadLeft(6, '0');

        var entity = new LoginCodeEntity()
        {
            Id = Guid.CreateVersion7(),
            ProviderId = provider.Id,
            UserId = user.Id,
            Code = code,
            ExpireDate = DateTime.UtcNow.AddMinutes(ExpirationMinutes),
            CreateDate = DateTime.UtcNow,
            UpdateDate = null,
        };

        await context.LoginCodes.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return code;
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, ProviderEntity provider, string code,
        CancellationToken cancellationToken = default)
    {
        if (provider.Name == ProviderTypes.Authenticator)
        {
            var userSecret = await secretManager.FindAsync(user, cancellationToken);

            if (userSecret is null)
            {
                return Results.NotFound("Not found user secret");
            }

            var secretBytes = Base32Encoding.ToBytes(userSecret.Secret);
            var totp = new Totp(secretBytes);
            var verified = totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);

            return !verified ? Results.BadRequest("Invalid token") : Result.Success();
        }

        var entity = await context.LoginCodes
            .Where(x => x.UserId == user.Id && x.ProviderId == provider.Id && x.Code == code)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            return Results.NotFound("Not found code");
        }

        if (entity.ExpireDate < DateTime.UtcNow)
        {
            return Results.BadRequest("Code expired");
        }

        await DeleteAsync(entity, cancellationToken);

        return Result.Success();
    }

    private async ValueTask<Result> DeleteAsync(LoginCodeEntity code, CancellationToken cancellationToken = default)
    {
        context.LoginCodes.Remove(code);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}