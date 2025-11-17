using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Oidc.Session;

public class SessionManager(
    AuthDbContext context,
    IOptions<SessionOptions> options) : ISessionManager
{
    private readonly AuthDbContext _context = context;
    private readonly SessionOptions _options = options.Value;

    public async ValueTask<SessionEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async ValueTask<SessionEntity?> FindAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        return await _context.Sessions.FirstOrDefaultAsync(x => x.DeviceId == device.Id, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        var session = new SessionEntity()
        {
            Id = Guid.CreateVersion7(),
            DeviceId = device.Id,
            IsActive = true,
            CreateDate = DateTimeOffset.UtcNow,
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