using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Cryptography.Keys;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Server.Security.Authentication.TwoFactor.Secret;

public sealed class SecretManager(
    AuthDbContext context,
    IKeyFactory keyFactory) : ISecretManager
{
    private readonly AuthDbContext _context = context;
    private readonly IKeyFactory _keyFactory = keyFactory;

    public async ValueTask<UserSecretEntity?> GetAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var userSecret = await _context.UserSecret.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
        return userSecret;
    }

    public async ValueTask<Result> AddAsync(UserSecretEntity secret,
        CancellationToken cancellationToken = default)
    {
        await _context.UserSecret.AddAsync(secret, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Success(SuccessCodes.Ok);
    }
    
    public async ValueTask<Result> UpdateAsync(UserSecretEntity secret,
        CancellationToken cancellationToken = default)
    {
        _context.UserSecret.Update(secret);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> RemoveAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var secret = await _context.UserSecret.FirstOrDefaultAsync(
            x => x.UserId == user.Id, cancellationToken);

        if (secret is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "Cannot find user secret or doesn't exists"
            });
        }

        _context.UserSecret.Remove(secret);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public string Generate() => _keyFactory.Create(20);
}