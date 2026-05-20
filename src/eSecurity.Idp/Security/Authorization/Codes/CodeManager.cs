using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Cryptography.Codes;
using eSecurity.Idp.Security.Cryptography.Hashing;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authorization.Codes;

public sealed class CodeManager(
    AuthDbContext context,
    IHasherProvider hasherProvider,
    ICodeFactory codeFactory) : ICodeManager
{
    private readonly AuthDbContext _context = context;
    private readonly ICodeFactory _codeFactory = codeFactory;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Pbkdf2);

    public async ValueTask<CodeEntity?> FindAsync(UserEntity user, 
        string code, CancellationToken cancellationToken = default)
    {
        var codes = await _context.Codes
            .Where(c => c.UserId == user.Id)
            .ToListAsync(cancellationToken);

        return codes.FirstOrDefault(c => _hasher.VerifyHash(code, c.CodeHash));
    }

    public async ValueTask<string> CreateAsync(UserEntity user, 
        SenderType sender, CancellationToken cancellationToken = default)
    {
        var code = _codeFactory.Create();
        var codeHash = _hasher.Hash(code);

        await _context.Codes.AddAsync(new CodeEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            CodeHash = codeHash,
            Sender = sender,
            State = CodeState.Pending,
            ExpiredAt = DateTime.UtcNow.AddMinutes(10)
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        return code;
    }

    public async ValueTask<Result> ConsumeAsync(CodeEntity code, CancellationToken cancellationToken = default)
    {
        code.ConsumedAt = DateTimeOffset.UtcNow;
        code.State = CodeState.Consumed;
        
        _context.Codes.Remove(code);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> CancelAsync(CodeEntity code, CancellationToken cancellationToken = default)
    {
        code.CancelledAt = DateTimeOffset.UtcNow;
        code.State = CodeState.Cancelled;
        
        _context.Codes.Remove(code);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}