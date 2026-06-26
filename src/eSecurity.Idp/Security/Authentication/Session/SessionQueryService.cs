using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.Session;

public sealed class SessionQueryService(AuthDbContext context) : ISessionQueryService
{
    private readonly AuthDbContext _context = context;
    
    public async ValueTask<SessionEntity?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions.FirstOrDefaultAsync(x => x.Id == sessionId, cancellationToken);
    }

    public async ValueTask<bool> HasClientAsync(Guid sessionId, Guid clientId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.ClientSessions
            .AnyAsync(x => x.SessionId == sessionId && x.ClientId == clientId, cancellationToken);
    }
}