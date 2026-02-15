using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Cryptography.Codes;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.Access.Codes;

public sealed class CodeManager(
    AuthDbContext context,
    IHasherProvider hasherProvider,
    ICodeFactory codeFactory) : ICodeManager
{
    private readonly AuthDbContext _context = context;
    private readonly ICodeFactory _codeFactory = codeFactory;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Pbkdf2);

    public async ValueTask<CodeEntity?> FindAsync(UserEntity user, 
        string codeHash, CancellationToken cancellationToken = default)
    {
        return await _context.Codes.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.CodeHash == codeHash, cancellationToken);
    }

    public async ValueTask<string> GenerateAsync(UserEntity user, SenderType sender, 
        ActionType action, PurposeType purpose, CancellationToken cancellationToken = default)
    {
        var code = _codeFactory.Create();
        var codeHash = _hasher.Hash(code);

        await _context.Codes.AddAsync(new CodeEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            CodeHash = codeHash,
            Action = action,
            Sender = sender,
            Purpose = purpose,
            ExpireDate = DateTime.UtcNow.AddMinutes(10)
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        return code;
    }

    public async ValueTask<Result> RemoveAsync(CodeEntity code, CancellationToken cancellationToken = default)
    {
        _context.Codes.Remove(code);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}