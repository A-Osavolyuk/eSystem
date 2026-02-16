using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Identity.User.Username;

public class UsernameManager(AuthDbContext context) : IUsernameManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> SetAsync(UserEntity user, string username,
        CancellationToken cancellationToken = default)
    {
        user.Username = username;
        user.UsernameChangeDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> ChangeAsync(UserEntity user, string username,
        CancellationToken cancellationToken = default)
    {
        user.Username = username;
        user.NormalizedUsername = username.ToUpper();
        user.UsernameChangeDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<bool> IsTakenAsync(string username, 
        CancellationToken cancellationToken = default)
    {
        var normalized = username.ToUpper();
        return await _context.Users.AnyAsync(u => u.NormalizedUsername == normalized, cancellationToken);
    }
}