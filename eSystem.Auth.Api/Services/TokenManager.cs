using System.Security.Claims;
using eSystem.Auth.Api.Security.Authentication.Tokens;
using eSystem.Auth.Api.Security.Authentication.Tokens.Jwt;
using eSystem.Auth.Api.Security.Cryptography.Keys;
using eSystem.Core.Attributes;
using eSystem.Core.Security.Cryptography.Keys;

namespace eSystem.Auth.Api.Services;

[Injectable(typeof(ITokenManager), ServiceLifetime.Scoped)]
public sealed class TokenManager(
    AuthDbContext context,
    IOptions<JwtOptions> options,
    IKeyFactory keyFactory,
    ITokenFactory tokenFactory) : ITokenManager
{
    private readonly AuthDbContext context = context;
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly ITokenFactory tokenFactory = tokenFactory;
    private readonly JwtOptions options = options.Value;

    public async Task<Result> SaveAsync(SessionEntity session, ClientEntity client,
        string refreshToken, CancellationToken cancellationToken = default)
    {
        var expirationDate = DateTimeOffset.UtcNow.AddDays(options.RefreshTokenExpirationDays);
        var token = await context.RefreshTokens.FirstOrDefaultAsync(
            token => token.SessionId == session.Id
            && token.ClientId == client.Id, cancellationToken);

        if (token is null)
        {
            token = new RefreshTokenEntity()
            {
                Id = Guid.CreateVersion7(),
                SessionId = session.Id,
                ClientId = client.Id,
                Token = refreshToken,
                ExpireDate = expirationDate,
                CreateDate = DateTimeOffset.UtcNow
            };

            await context.RefreshTokens.AddAsync(token, cancellationToken);
        }
        else
        {
            token.Token = refreshToken;
            token.ExpireDate = expirationDate;
            token.RefreshDate = DateTimeOffset.UtcNow;
            token.UpdateDate = DateTimeOffset.UtcNow;

            context.RefreshTokens.Update(token);
        }

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<RefreshTokenEntity?> FindAsync(string token, CancellationToken cancellationToken = default)
    {
        return await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    public async Task<Result> RemoveAsync(RefreshTokenEntity token, CancellationToken cancellationToken = default)
    {
        context.RefreshTokens.Remove(token);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims) => tokenFactory.Create(claims);

    public string GenerateRefreshToken(int length = 20) => keyFactory.Create(length);
}