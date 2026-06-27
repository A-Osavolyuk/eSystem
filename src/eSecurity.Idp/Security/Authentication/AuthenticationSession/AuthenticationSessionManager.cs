using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.AuthenticationSession;

public sealed class AuthenticationSessionManager(AuthDbContext context) : IAuthenticationSessionManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<AuthenticationSessionEntity?> FindByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuthenticationSessions
            .Include(x => x.AuthenticationMethods)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(AuthenticationSessionEntity entity,
        CancellationToken cancellationToken = default)
    {
        await _context.AuthenticationSessions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> UpdateAsync(AuthenticationSessionEntity entity,
        CancellationToken cancellationToken = default)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}