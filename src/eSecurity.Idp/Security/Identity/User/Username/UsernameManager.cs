using eSecurity.Idp.Common.Validation;
using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Identity.User.Username;

public class UsernameManager(AuthDbContext context) : IUsernameManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> SetAsync(UserEntity user, string username,
        CancellationToken cancellationToken = default)
    {
        user.Username = username;
        user.NormalizedUsername = Normalizer.Normalize(username);
        user.UsernameChangeDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> ChangeAsync(UserEntity user, string username, 
        CancellationToken cancellationToken = default)
    {
        user.Username = username;
        user.NormalizedUsername = Normalizer.Normalize(username);
        user.UsernameChangeDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<bool> IsTakenAsync(string username, CancellationToken cancellationToken = default)
    {
        var normalized = Normalizer.Normalize(username);
        return await _context.Users.AnyAsync(u => u.NormalizedUsername == normalized, cancellationToken);
    }
}