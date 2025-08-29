namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILoginSessionManager), ServiceLifetime.Scoped)]
public class LoginSessionManager(AuthDbContext context) : ILoginSessionManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask CreateAsync(UserDeviceEntity device, LoginStatus status, LoginType type,
        string provider, CancellationToken cancellationToken = default)
    {
        device.UpdateDate = DateTimeOffset.UtcNow;
        device.LastSeen = DateTimeOffset.UtcNow;
            
        var session = new LoginSessionEntity()
        {
            Id = Guid.CreateVersion7(),
            DeviceId = device.Id,
            IpAddress = device.IpAddress!,
            UserAgent = device.UserAgent!,
            Type = type,
            Status = status,
            Provider = provider,
            Timestamp = DateTimeOffset.UtcNow,
            CreateDate = DateTimeOffset.UtcNow
        };
            
        context.UserDevices.Update(device);
        await context.LoginSessions.AddAsync(session, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async ValueTask CreateAsync(UserDeviceEntity device, LoginStatus status, LoginType type,
        CancellationToken cancellationToken = default)
    {
        device.UpdateDate = DateTimeOffset.UtcNow;
        device.LastSeen = DateTimeOffset.UtcNow;
            
        var session = new LoginSessionEntity()
        {
            Id = Guid.CreateVersion7(),
            DeviceId = device.Id,
            IpAddress = device.IpAddress!,
            UserAgent = device.UserAgent!,
            Type = type,
            Status = status,
            Provider = null,
            Timestamp = DateTimeOffset.UtcNow,
            CreateDate = DateTimeOffset.UtcNow
        };
            
        context.UserDevices.Update(device);
        await context.LoginSessions.AddAsync(session, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}