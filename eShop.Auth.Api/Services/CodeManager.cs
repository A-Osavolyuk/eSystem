using eShop.Auth.Api.Security.Hashing;
using eShop.Auth.Api.Security.Protection;
using OtpNet;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ICodeManager), ServiceLifetime.Scoped)]
public sealed class CodeManager(
    AuthDbContext context,
    ISecretManager secretManager,
    SecretProtector protector,
    Hasher hasher) : ICodeManager
{
    private readonly AuthDbContext context = context;
    private readonly ISecretManager secretManager = secretManager;
    private readonly SecretProtector protector = protector;
    private readonly Hasher hasher = hasher;

    public async ValueTask<string> GenerateAsync(UserEntity user, SenderType sender, 
        CodeType type, CodeResource resource, CancellationToken cancellationToken = default)
    {
        var entity = await context.Codes
            .FirstOrDefaultAsync(x => x.UserId == user.Id 
                                      && x.Type == type 
                                      && x.Sender == sender, cancellationToken);

        if (entity is not null)
        {
            context.Codes.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }

        var code = CodeGenerator.Generate(6);
        var codeHash = hasher.Hash(code);

        await context.Codes.AddAsync(new CodeEntity()
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
        if (sender == SenderType.AuthenticatorApp)
        {
            var userSecret = await secretManager.FindAsync(user, cancellationToken);

            if (userSecret is null)
            {
                return Results.NotFound("Not found user secret");
            }

            var unprotectedSecret = protector.Unprotect(userSecret.Secret);
            var secretBytes = Base32Encoding.ToBytes(unprotectedSecret);
            var totp = new Totp(secretBytes);
            var isVerifiedCode = totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);

            return !isVerifiedCode ? Results.BadRequest("Invalid code") : Result.Success();
        }
        
        var entity = await context.Codes
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

        context.Codes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}