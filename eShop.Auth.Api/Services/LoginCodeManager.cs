using eShop.Auth.Api.Security.Hashing;
using eShop.Auth.Api.Security.Protection;
using eShop.Domain.Common.Security;
using eShop.Domain.Common.Security.Constants;
using OtpNet;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILoginCodeManager), ServiceLifetime.Scoped)]
public sealed class LoginCodeManager(
    AuthDbContext context,
    SecretProtector protector,
    ISecretManager secretManager,
    Hasher hasher) : ILoginCodeManager
{
    private readonly AuthDbContext context = context;
    private readonly ISecretManager secretManager = secretManager;
    private readonly Hasher hasher = hasher;
    private const int ExpirationMinutes = 30;

    public async ValueTask<string> GenerateAsync(UserEntity user, 
        TwoFactorProviderEntity twoFactorProvider, CancellationToken cancellationToken = default)
    {
        var existingCode = await context.LoginCodes
            .Where(x => x.UserId == user.Id && x.ProviderId == twoFactorProvider.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingCode is not null)
        {
            context.LoginCodes.Remove(existingCode);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        var code = CodeGenerator.Generate(6);
        var hash = hasher.Hash(code);
        
        var entity = new LoginCodeEntity()
        {
            Id = Guid.CreateVersion7(),
            ProviderId = twoFactorProvider.Id,
            UserId = user.Id,
            CodeHash = hash,
            ExpireDate = DateTime.UtcNow.AddMinutes(ExpirationMinutes),
            CreateDate = DateTime.UtcNow,
            UpdateDate = null,
        };

        await context.LoginCodes.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return code;
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, TwoFactorProviderEntity twoFactorProvider, string code,
        CancellationToken cancellationToken = default)
    {
        if (twoFactorProvider.Name == ProviderTypes.Authenticator)
        {
            var userSecret = await secretManager.FindAsync(user, cancellationToken);

            if (userSecret is null)
            {
                return Results.NotFound("Not found user secret");
            }

            var unprotectedSecret = protector.Unprotect(userSecret.Secret);
            var secretBytes = Base32Encoding.ToBytes(unprotectedSecret);
            var totp = new Totp(secretBytes);
            var isVerifiedCode = totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);

            return !isVerifiedCode ? Results.BadRequest("Invalid code") : Result.Success();
        }
        
        var entity = await context.LoginCodes
            .Where(x => x.UserId == user.Id && x.ProviderId == twoFactorProvider.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            return Results.NotFound("Not found code");
        }
        
        var isValidHash = hasher.VerifyHash(code, entity.CodeHash);

        if (!isValidHash)
        {
            return Results.BadRequest("Invalid code");
        }

        if (entity.ExpireDate < DateTime.UtcNow)
        {
            return Results.BadRequest("Code expired");
        }

        context.LoginCodes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}