using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.TwoFactor;

public sealed class TwoFactorManager(AuthDbContext context) : ITwoFactorManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> SubscribeAsync(UserEntity user, TwoFactorMethod method,
        bool preferred = false, CancellationToken cancellationToken = default)
    {
        if (preferred && user.TwoFactorMethods.Any(x => x.Preferred))
        {
            var preferredMethod = user.TwoFactorMethods.First(x => x.Preferred);
            preferredMethod.Preferred = false;
            preferredMethod.UpdateDate = DateTimeOffset.UtcNow;

            _context.UserTwoFactorMethods.Update(preferredMethod);
        }

        var userProvider = new UserTwoFactorMethodEntity()
        {
            UserId = user.Id,
            Method = method,
            Preferred = preferred,
            CreateDate = DateTimeOffset.UtcNow
        };

        if (!user.TwoFactorEnabled)
        {
            user.TwoFactorEnabled = true;
            user.UpdateDate = DateTimeOffset.UtcNow;
        }

        _context.Users.Update(user);
        await _context.UserTwoFactorMethods.AddAsync(userProvider, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UnsubscribeAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        user.TwoFactorEnabled = false;
        user.UpdateDate = DateTimeOffset.UtcNow;

        _context.Users.Update(user);
        _context.UserTwoFactorMethods.RemoveRange(user.TwoFactorMethods);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UnsubscribeAsync(UserTwoFactorMethodEntity method,
        CancellationToken cancellationToken = default)
    {
        _context.UserTwoFactorMethods.Remove(method);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> PreferAsync(UserEntity user,
        TwoFactorMethod method, CancellationToken cancellationToken = default)
    {
        if (user.TwoFactorMethods.Count == 1) return Results.BadRequest("Cannot change the only preferred method");

        var currentPreferredMethod = user.TwoFactorMethods.Single(x => x.Preferred);
        currentPreferredMethod.Preferred = false;
        currentPreferredMethod.UpdateDate = DateTimeOffset.UtcNow;
        
        var nextPreferredMethod = user.GetTwoFactorMethod(method)!;
        nextPreferredMethod.Preferred = true;
        nextPreferredMethod.UpdateDate = DateTimeOffset.UtcNow;
        
        _context.UserTwoFactorMethods.UpdateRange([currentPreferredMethod, nextPreferredMethod]);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }
}