using eSecurity.Data;
using eSecurity.Data.Entities;

namespace eSecurity.Security.Authentication.ODIC.Session;

public class SessionManager(
    AuthDbContext context,
    IOptions<SessionOptions> options) : ISessionManager
{
    private readonly AuthDbContext context = context;
    private readonly SessionOptions options = options.Value;

    public async ValueTask<SessionEntity?> FindAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        return await context.Sessions.FirstOrDefaultAsync(x => x.DeviceId == device.Id, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        var session = new SessionEntity()
        {
            Id = Guid.CreateVersion7(),
            DeviceId = device.Id,
            IsActive = true,
            CreateDate = DateTimeOffset.UtcNow,
            ExpireDate = DateTimeOffset.UtcNow.Add(options.Timestamp)
        };
        
        await context.Sessions.AddAsync(session, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RemoveAsync(SessionEntity session, CancellationToken cancellationToken = default)
    {
        context.Sessions.Remove(session);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}