using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Session;

public class SessionManager(
    AuthDbContext context,
    IOptions<SessionOptions> options) : ISessionManager
{
    private readonly AuthDbContext _context = context;
    private readonly SessionOptions _options = options.Value;

    public async ValueTask<SessionEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async ValueTask<SessionEntity?> FindAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions.FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var session = new SessionEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            IsActive = true,
            ExpireDate = DateTimeOffset.UtcNow.Add(_options.Timestamp)
        };
        
        await _context.Sessions.AddAsync(session, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> RemoveAsync(SessionEntity session, CancellationToken cancellationToken = default)
    {
        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}