using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Session;

public class SessionManager(
    AuthDbContext context,
    IOptions<SessionOptions> options) : ISessionManager
{
    private readonly AuthDbContext _context = context;
    private readonly SessionOptions _options = options.Value;

    public async ValueTask<bool> OwnClientAsync(SessionEntity session, 
        ClientEntity client, CancellationToken cancellationToken = default)
    {
        return await _context.ClientSessions.AnyAsync(
            x => x.SessionId == session.Id && x.ClientId == client.Id, cancellationToken);
    }

    public async ValueTask<SessionEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async ValueTask<SessionEntity?> FindAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(SessionEntity session, CancellationToken cancellationToken = default)
    {
        await _context.Sessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> RemoveAsync(SessionEntity session, CancellationToken cancellationToken = default)
    {
        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}