using eSecurity.Core.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authentication.TwoFactor.Recovery;

public sealed class RecoverManager(
    AuthDbContext context,
    IDataProtectionProvider protectionProvider,
    IRecoveryCodeFactory recoveryCodeFactory) : IRecoverManager
{
    private readonly AuthDbContext _context = context;
    private readonly IDataProtector _protector = protectionProvider.CreateProtector(ProtectionPurposes.RecoveryCode);
    private readonly IRecoveryCodeFactory _recoveryCodeFactory = recoveryCodeFactory;

    public List<string> Unprotect(UserEntity user)
    {
        return user.RecoveryCodes.Select(code => _protector.Unprotect(code.ProtectedCode)).ToList();
    }

    public async ValueTask<List<string>> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        if (user.RecoveryCodes.Count > 0) _context.UserRecoveryCodes.RemoveRange(user.RecoveryCodes);

        var codes = _recoveryCodeFactory.Create().ToList();
        var entities = codes
            .Select(code => _protector.Protect(code))
            .Select(hash => new UserRecoveryCodeEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                ProtectedCode = hash,
                CreateDate = DateTimeOffset.UtcNow
            })
            .ToList();

        await _context.UserRecoveryCodes.AddRangeAsync(entities, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return codes;
    }

    public async ValueTask<Result> VerifyAsync(UserEntity user, string code,
        CancellationToken cancellationToken = default)
    {
        if (user.RecoveryCodes.Count == 0) return Results.BadRequest("User does not have any recovery code.");
        
        var entity = user.RecoveryCodes.FirstOrDefault(
            x => _protector.Unprotect(x.ProtectedCode) == code);
        
        if (entity is null) return Results.BadRequest("Invalid recovery code.");

        _context.UserRecoveryCodes.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RevokeAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var codes = await _context.UserRecoveryCodes
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
        
        _context.UserRecoveryCodes.RemoveRange(codes);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}