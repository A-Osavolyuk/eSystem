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

    public async ValueTask<List<string>> UnprotectAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        return await _context.UserRecoveryCodes
            .Where(x => x.UserId == user.Id)
            .Select(code => _protector.Unprotect(code.ProtectedCode))
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<List<string>> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var recoveryCodes = await _context.UserRecoveryCodes
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
        
        if (recoveryCodes.Count > 0) _context.UserRecoveryCodes.RemoveRange(recoveryCodes);

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
        var recoveryCodes = await _context.UserRecoveryCodes
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
        
        if (recoveryCodes.Count == 0) return Results.BadRequest("User does not have any recovery code.");
        
        var recoveryCode = recoveryCodes.FirstOrDefault(
            x => _protector.Unprotect(x.ProtectedCode) == code);
        
        if (recoveryCode is null) return Results.BadRequest("Invalid recovery code.");

        _context.UserRecoveryCodes.Remove(recoveryCode);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> RevokeAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var codes = await _context.UserRecoveryCodes
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);

        _context.UserRecoveryCodes.RemoveRange(codes);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<bool> HasAsync(UserEntity user, CancellationToken cancellationToken = default)
        => await _context.UserRecoveryCodes.AnyAsync(x => x.UserId == user.Id, cancellationToken);
}