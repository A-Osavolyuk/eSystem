using eShop.Auth.Api.Security.Hashing;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IVerificationCodeManager), ServiceLifetime.Scoped)]
public sealed class VerificationVerificationCodeManager(
    AuthDbContext context,
    Hasher hasher) : IVerificationCodeManager
{
    private readonly AuthDbContext context = context;
    private readonly Hasher hasher = hasher;

    public async ValueTask<string> GenerateAsync(UserEntity user, SenderType sender, 
        CodeType type, CodeResource resource, CancellationToken cancellationToken = default)
    {
        var entity = await context.VerificationCodes
            .FirstOrDefaultAsync(x => x.UserId == user.Id 
                                      && x.Type == type 
                                      && x.Sender == sender, cancellationToken);

        if (entity is not null)
        {
            context.VerificationCodes.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }

        var code = CodeGenerator.Generate(6);
        var codeHash = hasher.Hash(code);

        await context.VerificationCodes.AddAsync(new VerificationCodeEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            CodeHash = codeHash,
            Type = type,
            Sender = sender,
            Resource = resource,
            CreateDate = DateTime.UtcNow,
            ExpireDate = DateTime.UtcNow.AddMinutes(10)
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        return code;
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, string code, SenderType sender, CodeType type,
        CodeResource resource, CancellationToken cancellationToken = default)
    {
        var entity = await context.VerificationCodes
            .SingleOrDefaultAsync(x => x.UserId == user.Id
                                       && x.Type == type
                                       && x.Sender == sender
                                       && x.Resource == resource
                                       && x.ExpireDate > DateTime.UtcNow, cancellationToken: cancellationToken);

        if (entity is null)
        {
            return Results.NotFound("Code not found");
        }
        
        var isValidHash = hasher.VerifyHash(code, entity.CodeHash);

        if (!isValidHash)
        {
            return Results.BadRequest("Invalid code");
        }

        context.VerificationCodes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}