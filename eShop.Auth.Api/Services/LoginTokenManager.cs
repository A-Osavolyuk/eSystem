using eShop.Domain.Common.Security;
using OtpNet;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILoginTokenManager), ServiceLifetime.Scoped)]
public sealed class LoginTokenManager(
    AuthDbContext context,
    ISecretManager secretManager) : ILoginTokenManager
{
    private readonly AuthDbContext context = context;
    private readonly ISecretManager secretManager = secretManager;
    private const int ExpirationMinutes = 30;

    public async ValueTask<string> GenerateAsync(UserEntity user, 
        ProviderEntity provider, CancellationToken cancellationToken = default)
    {
        var existingToken = await FindAsync(user, provider, cancellationToken);

        if (existingToken is not null)
        {
            if (existingToken.ExpireDate < DateTime.UtcNow)
            {
                return existingToken.Code;
            }

            await DeleteAsync(existingToken, cancellationToken);
        }

        var token = GenerateToken();

        var entity = new LoginCodeEntity()
        {
            Id = Guid.CreateVersion7(),
            ProviderId = provider.Id,
            UserId = user.Id,
            Code = token,
            ExpireDate = DateTime.UtcNow.AddMinutes(ExpirationMinutes),
            CreateDate = DateTime.UtcNow,
            UpdateDate = null,
        };

        await context.LoginCodes.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return token;
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

            var isTokenVerified = VerifyToken(userSecret.Secret, code);

            return !isTokenVerified ? Results.BadRequest("Invalid token") : Result.Success();
        }

        var entity = await FindAsync(user, provider, cancellationToken);

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

    private async ValueTask<Result> DeleteAsync(LoginCodeEntity code, CancellationToken cancellationToken = default)
    {
        context.LoginCodes.Remove(code);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
    
    private async ValueTask<LoginCodeEntity?> FindAsync(UserEntity user, ProviderEntity provider, CancellationToken cancellationToken = default)
    {
        var entity = await context.LoginCodes
            .Where(x => x.UserId == user.Id && x.ProviderId == provider.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return entity;
    }
    
    private string GenerateToken()
    {
        var randomCode = new Random().Next(0, 999999).ToString();
        var token = randomCode.PadLeft(6, '0');
        return token;
    }
    
    private bool VerifyToken(string secret, string token)
    {
        var secretBytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(secretBytes);
        
        return totp.VerifyTotp(token, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}