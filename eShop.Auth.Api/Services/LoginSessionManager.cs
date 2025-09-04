namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILoginSessionManager), ServiceLifetime.Scoped)]
public class LoginSessionManager(AuthDbContext context) : ILoginSessionManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask CreateAsync(UserDeviceEntity device, LoginType type,
        string provider, CancellationToken cancellationToken = default)
    {
        device.UpdateDate = DateTimeOffset.UtcNow;
        device.LastSeen = DateTimeOffset.UtcNow;
            
        var session = new LoginSessionEntity()
        {
            Id = Guid.CreateVersion7(),
            DeviceId = device.Id,
            Type = type,
            Provider = provider,
            Timestamp = DateTimeOffset.UtcNow,
            CreateDate = DateTimeOffset.UtcNow
        };
            
        context.UserDevices.Update(device);
        await context.LoginSessions.AddAsync(session, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask CreateAsync(UserDeviceEntity device, LoginType type,
        CancellationToken cancellationToken = default)
    {
        device.UpdateDate = DateTimeOffset.UtcNow;
        device.LastSeen = DateTimeOffset.UtcNow;
            
        var session = new LoginSessionEntity()
        {
            Id = Guid.CreateVersion7(),
            DeviceId = device.Id,
            Type = type,
            Provider = null,
            Timestamp = DateTimeOffset.UtcNow,
            CreateDate = DateTimeOffset.UtcNow
        };
            
        context.UserDevices.Update(device);
        await context.LoginSessions.AddAsync(session, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}