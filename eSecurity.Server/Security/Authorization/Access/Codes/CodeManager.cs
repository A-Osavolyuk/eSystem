using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Cryptography.Codes;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSystem.Core.Common.Messaging;

namespace eSecurity.Server.Security.Authorization.Access.Codes;

public sealed class CodeManager(
    AuthDbContext context,
    IHasherFactory hasherFactory,
    ICodeFactory codeFactory) : ICodeManager
{
    private readonly AuthDbContext context = context;
    private readonly ICodeFactory codeFactory = codeFactory;
    private readonly Hasher hasher = hasherFactory.Create(HashAlgorithm.Pbkdf2);

    public async ValueTask<string> GenerateAsync(UserEntity user, SenderType sender, 
        ActionType action, PurposeType purpose, CancellationToken cancellationToken = default)
    {
        var entity = await context.Codes
            .FirstOrDefaultAsync(x => x.UserId == user.Id 
                                      && x.Action == action 
                                      && x.Sender == sender, cancellationToken);

        if (entity is not null)
        {
            context.Codes.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }

        var code = codeFactory.Create();
        var codeHash = hasher.Hash(code);

        await context.Codes.AddAsync(new CodeEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            CodeHash = codeHash,
            Action = action,
            Sender = sender,
            Purpose = purpose,
            CreateDate = DateTime.UtcNow,
            ExpireDate = DateTime.UtcNow.AddMinutes(10)
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        return code;
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, string code, SenderType sender, ActionType action,
        PurposeType purpose, CancellationToken cancellationToken = default)
    {
        var entity = await context.Codes.SingleOrDefaultAsync(
                x => x.UserId == user.Id && x.Action == action && x.Sender == sender
                && x.Purpose == purpose && x.ExpireDate > DateTime.UtcNow, cancellationToken);

        if (entity is null) return Results.NotFound("Code not found");
        
        var isValidHash = hasher.VerifyHash(code, entity.CodeHash);
        if (!isValidHash) return Results.BadRequest("Invalid code");

        context.Codes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}