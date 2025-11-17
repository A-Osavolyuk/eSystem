using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Oidc.Token;

public sealed class TokenManager(
    AuthDbContext context) : ITokenManager
{
    private readonly AuthDbContext _context = context;

    public async Task<Result> CreateAsync(RefreshTokenEntity token, 
        CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(token, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }

    public async Task<RefreshTokenEntity?> FindByTokenAsync(string token, 
        CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
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
        
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async Task<Result> RemoveAsync(RefreshTokenEntity token, CancellationToken cancellationToken = default)
    {
        _context.RefreshTokens.Remove(token);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}