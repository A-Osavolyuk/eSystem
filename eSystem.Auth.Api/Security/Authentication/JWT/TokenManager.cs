using System.Security.Claims;
using eSystem.Auth.Api.Data.Entities;
using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Cryptography.Keys;
using eSystem.Core.Security.Cryptography.Tokens;

namespace eSystem.Auth.Api.Security.Authentication.JWT;

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

    public async Task<Result> CreateAsync(RefreshTokenEntity token, 
        CancellationToken cancellationToken = default)
    {
        await context.RefreshTokens.AddAsync(token, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<RefreshTokenEntity?> FindByTokenAsync(string token, 
        CancellationToken cancellationToken = default)
    {
        return await context.RefreshTokens
            .Include(x => x.Session)
            .ThenInclude(x => x.Device)
            .Include(x => x.Client)
            .ThenInclude(x => x.AllowedScopes)
            .ThenInclude(x => x.Scope)
            .Include(x => x.Client)
            .ThenInclude(x => x.RedirectUris)
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    public async Task<Result> RotateAsync(RefreshTokenEntity revokedToken, 
        RefreshTokenEntity newToken, CancellationToken cancellationToken = default)
    {
        revokedToken.Revoked = true;
        revokedToken.RevokeDate = DateTimeOffset.UtcNow;
        revokedToken.UpdateDate = DateTimeOffset.UtcNow;
        
        await context.RefreshTokens.AddAsync(newToken, cancellationToken);
        context.RefreshTokens.Update(revokedToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveAsync(RefreshTokenEntity token, CancellationToken cancellationToken = default)
    {
        context.RefreshTokens.Remove(token);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims, string audience) 
        => tokenFactory.Create(claims, audience);

    public string GenerateRefreshToken(int length = 20) => keyFactory.Create(length);
}