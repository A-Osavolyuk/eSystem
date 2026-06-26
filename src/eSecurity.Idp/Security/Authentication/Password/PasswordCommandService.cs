using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Cryptography.Hashing;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.Password;

public sealed class PasswordCommandService(
    AuthDbContext context,
    IPasswordQueryService query,
    IHasherProvider hasherProvider) : IPasswordCommandService
{
    private readonly AuthDbContext _context = context;
    private readonly IPasswordQueryService _query = query;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Pbkdf2);

    public async ValueTask<Result> AddAsync(Guid userId, string password, 
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);
        
        if (await _query.ExistsAsync(userId, cancellationToken))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "User already has password"
            });
        }

        var entity = new PasswordEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Hash = _hasher.Hash(password)
        };

        await _context.Passwords.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Created);
    }

    public async ValueTask<Result> ChangeAsync(Guid userId, string currentPassword, string newPassword,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currentPassword);
        ArgumentException.ThrowIfNullOrWhiteSpace(newPassword);

        var currentPasswordEntity = await _query.GetByUserAsync(userId, cancellationToken);
        if (currentPasswordEntity is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "User does not have password yet"
            });
        }

        if (_hasher.VerifyHash(currentPassword, currentPasswordEntity.Hash))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid current password"
            });
        }

        currentPasswordEntity.Hash = _hasher.Hash(newPassword);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> ResetAsync(Guid userId, string password,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var currentPassword = await _query.GetByUserAsync(userId, cancellationToken);
        if (currentPassword is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "User does not have password yet"
            });
        }

        currentPassword.Hash = _hasher.Hash(password);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> VerifyAsync(Guid userId, string password, 
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var currentPassword = await _query.GetByUserAsync(userId, cancellationToken);
        if (currentPassword is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "User does not have password yet"
            });
        }
        
        if (_hasher.VerifyHash(password, currentPassword.Hash))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid password"
            });
        }

        return Results.Success(SuccessCodes.Ok);
    }
}