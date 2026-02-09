using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.Session;

public sealed class OAuthSessionManager(AuthDbContext context) : IOAuthSessionManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<OAuthSessionEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken)
        => await _context.OAuthSessions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async ValueTask<Result> CreateAsync(OAuthSessionEntity session, CancellationToken cancellationToken)
    {
        await _context.OAuthSessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UpdateAsync(OAuthSessionEntity session, CancellationToken cancellationToken)
    {
        _context.OAuthSessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}