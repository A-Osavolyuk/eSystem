using eSystem.Auth.Api.Data;
using eSystem.Auth.Api.Entities;
using eSystem.Auth.Api.Interfaces;
using eSystem.Domain.Security.Authentication;

namespace eSystem.Auth.Api.Services;

[Injectable(typeof(ILoginManager), ServiceLifetime.Scoped)]
public class LoginManager(AuthDbContext context) : ILoginManager
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