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
        var existingCode = await context.LoginCodes
            .Where(x => x.UserId == user.Id && x.ProviderId == provider.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingCode is not null)
        {
            context.LoginCodes.Remove(existingCode);
            await context.SaveChangesAsync(cancellationToken);
        }

        var userSecret = await secretManager.FindAsync(user, cancellationToken);
        
        if (userSecret is null)
        {
            throw new Exception("Secret not generated");
        }
        
        var secretBytes = Base32Encoding.ToBytes(userSecret.Secret);
        var code = new Totp(secretBytes).ComputeTotp();
        var hash = Pbkdf2Hasher.Hash(code);

        var entity = new LoginCodeEntity()
        {
            Id = Guid.CreateVersion7(),
            ProviderId = provider.Id,
            UserId = user.Id,
            Hash = hash,
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
        var entity = await context.LoginCodes
            .Where(x => x.UserId == user.Id && x.ProviderId == provider.Id && x.Hash == code)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity is null)
        {
            return Results.NotFound("Not found code");
        }
        
        var userSecret = await secretManager.FindAsync(user, cancellationToken);

        if (userSecret is null)
        {
            return Results.NotFound("Not found user secret");
        }

        var secretBytes = Base32Encoding.ToBytes(userSecret.Secret);
        var totp = new Totp(secretBytes);
        var isVerifiedCode = totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
        var isValidHash = Pbkdf2Hasher.VerifyHash(code, entity.Hash);

        if (!isValidHash || isVerifiedCode)
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