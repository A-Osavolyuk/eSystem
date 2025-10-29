using eSystem.Auth.Api.Data.Entities;
using eSystem.Core.Attributes;
using eSystem.Core.Security.Cryptography.Protection;

namespace eSystem.Auth.Api.Security.Authentication.TwoFactor.Recovery;

[Injectable(typeof(IRecoverManager), ServiceLifetime.Scoped)]
public sealed class RecoverManager(
    AuthDbContext context,
    IProtectorFactory protectorFactory,
    IRecoveryCodeFactory recoveryCodeFactory) : IRecoverManager
{
    private readonly AuthDbContext context = context;
    private readonly IRecoveryCodeFactory recoveryCodeFactory = recoveryCodeFactory;
    private readonly IProtector protector = protectorFactory.Create(ProtectionPurposes.RecoveryCode);

    public List<string> Unprotect(UserEntity user)
    {
        return user.RecoveryCodes.Select(code => protector.Unprotect(code.ProtectedCode)).ToList();
    }

    public async ValueTask<List<string>> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        if (user.RecoveryCodes.Count > 0) context.UserRecoveryCodes.RemoveRange(user.RecoveryCodes);

        var codes = recoveryCodeFactory.Create().ToList();
        var entities = codes
            .Select(code => protector.Protect(code))
            .Select(hash => new UserRecoveryCodeEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                ProtectedCode = hash,
                CreateDate = DateTimeOffset.UtcNow
            })
            .ToList();

        await context.UserRecoveryCodes.AddRangeAsync(entities, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return codes;
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, string code,
        CancellationToken cancellationToken = default)
    {
        if (user.RecoveryCodes.Count == 0) return Results.BadRequest("User does not have any recovery code.");
        
        var entity = user.RecoveryCodes.FirstOrDefault(
            x => protector.Unprotect(x.ProtectedCode) == code);
        
        if (entity is null) return Results.BadRequest("Invalid recovery code.");

        context.UserRecoveryCodes.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RevokeAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var codes = await context.UserRecoveryCodes
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
        
        context.UserRecoveryCodes.RemoveRange(codes);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}