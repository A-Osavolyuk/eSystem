using eSecurity.Data.Entities;
using eSecurity.Security.Cryptography.Tokens.Jwt;
using eSystem.Core.Security.Authentication.Jwt;

namespace eSecurity.Security.Authentication.Odic.Token;

public sealed class TokenManager(
    AuthDbContext context) : ITokenManager
{
    private readonly AuthDbContext context = context;

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
            .FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    public async Task<Result> RevokeAsync(RefreshTokenEntity token, 
        CancellationToken cancellationToken = default)
    {
        token.Revoked = true;
        token.RevokeDate = DateTimeOffset.UtcNow;
        token.UpdateDate = DateTimeOffset.UtcNow;
        
        context.RefreshTokens.Update(token);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Result> RemoveAsync(RefreshTokenEntity token, CancellationToken cancellationToken = default)
    {
        context.RefreshTokens.Remove(token);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}