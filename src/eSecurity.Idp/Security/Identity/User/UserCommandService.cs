using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Idp.Common.Validation;
using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Identity.User;

public sealed class UserCommandService(AuthDbContext context, IUserQueryService query) : IUserCommandService
{
    private readonly AuthDbContext _context = context;
    private readonly IUserQueryService _query = query;

    public async ValueTask<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(user);

        var lockoutState = new UserLockoutStateEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Type = LockoutType.None,
        };
        
        await _context.Users.AddAsync(user, cancellationToken);
        await _context.LockoutStates.AddAsync(lockoutState, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> ChangeUsernameAsync(Guid userId, string username, 
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        var user = await _query.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error()
            {
                Code = ErrorCode.NotFound,
                Description = "User not found"
            });
        }

        user.Username = username;
        user.NormalizedUsername = Normalizer.Normalize(username);
        user.UsernameChangedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}