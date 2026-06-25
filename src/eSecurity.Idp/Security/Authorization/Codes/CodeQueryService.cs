using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Cryptography.Hashing;

namespace eSecurity.Idp.Security.Authorization.Codes;

public sealed class CodeQueryService(
    AuthDbContext context,
    IHasherProvider hasherProvider) : ICodeQueryService
{
    private readonly AuthDbContext _context = context;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Pbkdf2);

    public async ValueTask<CodeEntity?> GetByCodeAsync(Guid userId, string code, 
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(code);

        var codeHash = _hasher.Hash(code);
        return await _context.Codes.FirstOrDefaultAsync(
            x => x.UserId == userId && x.CodeHash == codeHash, cancellationToken);
    }
}