using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Cryptography.Hashing;

namespace eSecurity.Idp.Security.Authorization.Token;

public sealed class TokenQueryService(
    AuthDbContext context,
    IHasherProvider hasherProvider) : ITokenQueryService
{
    private readonly AuthDbContext _context = context;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Sha512);

    public async ValueTask<bool> ExistsAsync(string token, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var tokenHash = _hasher.Hash(token);
        return await _context.OpaqueTokens.AnyAsync(x => x.TokenHash == tokenHash, cancellationToken);
    }

    public async ValueTask<OpaqueTokenEntity?> GetByIdAsync(Guid tokenId, CancellationToken cancellationToken = default)
    {
        return await _context.OpaqueTokens
            .Include(x => x.Audiences)
            .Include(x => x.Scopes)
            .ThenInclude(x => x.ClientScope)
            .ThenInclude(x => x.Scope)
            .FirstOrDefaultAsync(x => x.Id == tokenId, cancellationToken);
    }

    public async ValueTask<OpaqueTokenEntity?> GetByTokenAsync(string token,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var tokenHash = _hasher.Hash(token);
        return await _context.OpaqueTokens
            .Include(x => x.Audiences)
            .Include(x => x.Scopes)
            .ThenInclude(x => x.ClientScope)
            .ThenInclude(x => x.Scope)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
    }

    public async ValueTask<OpaqueTokenEntity?> GetByTokenAsync(string token, OpaqueTokenType type,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var tokenHash = _hasher.Hash(token);
        return await _context.OpaqueTokens
            .Include(x => x.Audiences)
            .Include(x => x.Scopes)
            .ThenInclude(x => x.ClientScope)
            .ThenInclude(x => x.Scope)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash && x.TokenType == type, cancellationToken);
    }
}