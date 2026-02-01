using eCinema.Server.Data;
using eCinema.Server.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace eCinema.Server.Security.Authentication.OpenIdConnect.Session;

public class SessionManager(AppDbContext context) : ISessionManager
{
    private readonly AppDbContext _context = context;

    public async ValueTask<SessionEntity?> FindBySidAsync(string sid, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .Where(s => s.Sid == sid)
            .Include(s => s.Claims)
            .Include(s => s.Properties)
            .Include(s => s.Tokens)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<SessionEntity?> FindByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .Where(s => s.SessionKey == key)
            .Include(s => s.Claims)
            .Include(s => s.Properties)
            .Include(s => s.Tokens)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(SessionEntity session, CancellationToken cancellationToken = default)
    {
        await _context.Sessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }

    public async ValueTask<Result> UpdateAsync(SessionEntity session, CancellationToken cancellationToken = default)
    {
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }

    public async ValueTask<Result> DeleteAsync(SessionEntity session, CancellationToken cancellationToken = default)
    {
        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync(cancellationToken);
        return Results.Ok();
    }
}