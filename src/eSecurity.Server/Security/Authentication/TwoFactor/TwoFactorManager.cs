using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.TwoFactor;

public sealed class TwoFactorManager(AuthDbContext context) : ITwoFactorManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<UserTwoFactorMethodEntity>> GetAllAsync(UserEntity user, 
        CancellationToken cancellationToken = default)
    {
        return await _context.UserTwoFactorMethods
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<UserTwoFactorMethodEntity?> GetAsync(UserEntity user,
        TwoFactorMethod method, CancellationToken cancellationToken = default)
    {
        return await _context.UserTwoFactorMethods.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Method == method, cancellationToken);
    }

    public async ValueTask<UserTwoFactorMethodEntity?> GetPreferredAsync(UserEntity user, 
        CancellationToken cancellationToken = default)
    {
        return await _context.UserTwoFactorMethods.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.Preferred, cancellationToken);
    }

    public async ValueTask<Result> SubscribeAsync(UserEntity user, TwoFactorMethod method,
        bool preferred = false, CancellationToken cancellationToken = default)
    {
        var preferredMethod = await GetPreferredAsync(user, cancellationToken);
        if (preferred && preferredMethod is not null)
        {
            preferredMethod.Preferred = false;

            _context.UserTwoFactorMethods.Update(preferredMethod);
        }

        var userProvider = new UserTwoFactorMethodEntity()
        {
            UserId = user.Id,
            Method = method,
            Preferred = preferred,
        };

        await _context.UserTwoFactorMethods.AddAsync(userProvider, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UnsubscribeAsync(UserEntity user,
        CancellationToken cancellationToken = default)
    {
        var methods = await GetAllAsync(user, cancellationToken);
        
        _context.UserTwoFactorMethods.RemoveRange(methods);
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
        if (await _context.UserTwoFactorMethods.CountAsync(
                x => x.UserId == user.Id, cancellationToken) == 1)
            return Results.BadRequest("Cannot change the only preferred method");

        var currentPreferredMethod = await GetPreferredAsync(user, cancellationToken);
        if (currentPreferredMethod is null) return Results.BadRequest("Invalid method");
        
        currentPreferredMethod.Preferred = false;

        var nextPreferredMethod = await GetAsync(user, method, cancellationToken);
        if (nextPreferredMethod is null) return Results.NotFound("Method not found");

        nextPreferredMethod.Preferred = true;

        _context.UserTwoFactorMethods.UpdateRange([currentPreferredMethod, nextPreferredMethod]);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<bool> HasMethodAsync(UserEntity user, TwoFactorMethod method,
        CancellationToken cancellationToken = default) => await _context.UserTwoFactorMethods.AnyAsync(
        x => x.UserId == user.Id && x.Method == method, cancellationToken);

    public async ValueTask<bool> IsEnabledAsync(UserEntity user, CancellationToken cancellationToken = default)
        => await _context.UserTwoFactorMethods.AnyAsync(
            x => x.UserId == user.Id && x.Method == TwoFactorMethod.AuthenticatorApp, cancellationToken);
}