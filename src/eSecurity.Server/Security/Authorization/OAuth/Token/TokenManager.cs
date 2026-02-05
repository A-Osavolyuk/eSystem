using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.Token;

public sealed class TokenManager(
    AuthDbContext context) : ITokenManager
{
    private readonly AuthDbContext _context = context;

    public async Task<OpaqueTokenEntity?> FindByHashAsync(string hash, OpaqueTokenType type, 
        CancellationToken cancellationToken = default)
    {
        return await _context.OpaqueTokens
            .Where(x => x.TokenHash == hash && x.TokenType == type)
            .Include(x => x.Client)
            .Include(x => x.Scopes)
            .ThenInclude(x => x.ClientScope)
            .FirstOrDefaultAsync(cancellationToken);
    }
    
    public async Task<OpaqueTokenEntity?> FindByHashAsync(string hash,
        CancellationToken cancellationToken = default)
    {
        return await _context.OpaqueTokens
            .Where(x => x.TokenHash == hash)
            .Include(x => x.Client)
            .Include(x => x.Scopes)
            .ThenInclude(x => x.ClientScope)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Result> CreateAsync(OpaqueTokenEntity token, IEnumerable<ClientAllowedScopeEntity> scopes,
        CancellationToken cancellationToken = default)
    {
        var tokenScopes = scopes
            .Select(s => new OpaqueTokenScopeEntity
            {
                Id = Guid.CreateVersion7(),
                TokenId = token.Id,
                ClientScopeId = s.Id
            }).ToList();

        await _context.OpaqueTokens.AddAsync(token, cancellationToken);
        await _context.OpaqueTokensScopes.AddRangeAsync(tokenScopes, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async Task<bool> IsOpaqueAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.OpaqueTokens.AnyAsync(x => x.TokenHash == token, cancellationToken);
    }

    public async Task<Result> RevokeAsync(OpaqueTokenEntity token,
        CancellationToken cancellationToken = default)
    {
        token.Revoked = true;
        token.RevokedDate = DateTimeOffset.UtcNow;

        _context.OpaqueTokens.Update(token);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async Task<Result> RemoveAsync(OpaqueTokenEntity token, CancellationToken cancellationToken = default)
    {
        _context.OpaqueTokens.Remove(token);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}