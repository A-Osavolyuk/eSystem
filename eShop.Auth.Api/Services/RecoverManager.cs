
using eShop.Auth.Api.Security.Protection;
using OtpNet;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(IRecoverManager), ServiceLifetime.Scoped)]
public sealed class RecoverManager(
    AuthDbContext context,
    CodeProtector codeProtector) : IRecoverManager
{
    private readonly AuthDbContext context = context;
    private readonly CodeProtector codeProtector = codeProtector;
    private const int CodesAmount = 16;
    private const int CodesLength = 10;

    public List<string> Unprotect(UserEntity user)
    {
        return user.RecoveryCodes.Select(code => codeProtector.Unprotect(code.ProtectedCode)).ToList();
    }

    public async ValueTask<List<string>> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        if (user.RecoveryCodes.Count > 0) context.UserRecoveryCodes.RemoveRange(user.RecoveryCodes);

        var codes = Generate();
        var entities = codes
            .Select(code => codeProtector.Protect(code))
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
            x => codeProtector.Unprotect(x.ProtectedCode) == code);
        
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

    private List<string> Generate()
    {
        var codes = new List<string>();
        for (var i = 0; i < CodesAmount; i++)
        {
            var keyBytes = KeyGeneration.GenerateRandomKey(CodesLength);
            var keyString = Base32Encoding.ToString(keyBytes);
            var key = keyString[..CodesLength]!;
            codes.Add(key);
        }
        
        return codes;
    }
}