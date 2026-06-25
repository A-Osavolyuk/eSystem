using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Cryptography;
using eSecurity.Idp.Security.Cryptography.Hashing;
using eSystem.Core.Messaging;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authorization.Codes;

public sealed class CodeCommandService(AuthDbContext context, IHasherProvider hasherProvider) : ICodeCommandService
{
    private readonly AuthDbContext _context = context;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Pbkdf2);

    public async ValueTask<TypedResult<string>> CreateAsync(Guid userId, SenderType sender,
        CancellationToken cancellationToken = default)
    {
        var code = CodeFactory.Create();
        var codeHash = _hasher.Hash(code);

        var entity = new CodeEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Sender = sender,
            State = CodeState.Pending,
            CodeHash = codeHash,
            ExpiredAt = DateTimeOffset.UtcNow.AddMinutes(10)
        };

        await _context.Codes.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return TypedResult<string>.Success(code);
    }

    public async ValueTask<Result> ConsumeAsync(CodeEntity code, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        code.State = CodeState.Consumed;
        code.ConsumedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> CancelAsync(CodeEntity code, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(code);

        code.State = CodeState.Cancelled;
        code.CancelledAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}