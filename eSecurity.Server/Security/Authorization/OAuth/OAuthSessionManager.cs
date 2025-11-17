using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.OAuth;

public class OAuthSessionManager(AuthDbContext context) : IOAuthSessionManager
{
    private readonly AuthDbContext _context = context;
    
    public async ValueTask<OAuthSessionEntity?> FindAsync(Guid id, string token, CancellationToken cancellationToken = default)
    {
        return await _context.OAuthSessions
            .Where(x => x.Id == id && x.Token == token)
            .Include(x => x.LinkedAccount)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(OAuthSessionEntity session, CancellationToken cancellationToken = default)
    {
        await _context.OAuthSessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UpdateAsync(OAuthSessionEntity session, CancellationToken cancellationToken = default)
    {
        session.UpdateDate = DateTimeOffset.UtcNow;
        
        _context.OAuthSessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}