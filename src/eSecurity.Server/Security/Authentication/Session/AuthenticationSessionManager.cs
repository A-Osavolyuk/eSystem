using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Session;

public sealed class AuthenticationSessionManager(AuthDbContext context) : IAuthenticationSessionManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<AuthenticationSessionEntity?> FindByIdAsync(Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.AuthenticationSessions.FirstOrDefaultAsync(
            x => x.Id == id, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(AuthenticationSessionEntity entity,
        CancellationToken cancellationToken = default)
    {
        await _context.AuthenticationSessions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UpdateAsync(AuthenticationSessionEntity entity,
        CancellationToken cancellationToken = default)
    {
        _context.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }
}