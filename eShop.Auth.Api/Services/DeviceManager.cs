namespace eShop.Auth.Api.Services;

[Injectable(typeof(IDeviceManager), ServiceLifetime.Scoped)]
public class DeviceManager(AuthDbContext context) : IDeviceManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<UserDeviceEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var device = await context.UserDevices.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return device;
    }

    public async ValueTask<UserDeviceEntity?> FindAsync(UserEntity user, string userAgent, 
        string ipAddress, CancellationToken cancellationToken = default)
    {
        var device = await context.UserDevices.FirstOrDefaultAsync(
            x => x.UserId == user.Id 
                 && x.UserAgent == userAgent 
                 && x.IpAddress == ipAddress, cancellationToken);

        return device;
    }

    public async ValueTask<Result> CreateAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        if (device.Device == "Other")
        {
            device.Device = "Desktop";
        }
        
        await context.UserDevices.AddAsync(device, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }

    public async ValueTask<Result> TrustAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        device.IsTrusted = true;
        device.UpdateDate = DateTimeOffset.UtcNow;
        
        context.UserDevices.Update(device);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> DistrustAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        device.IsTrusted = false;
        device.UpdateDate = DateTimeOffset.UtcNow;
        
        context.UserDevices.Update(device);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> BlockAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        device.IsBlocked = true;
        device.BlockedDate = DateTimeOffset.UtcNow;
        device.UpdateDate = DateTimeOffset.UtcNow;
        
        context.UserDevices.Update(device);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UnblockAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        device.IsBlocked = false;
        device.BlockedDate = null;
        device.UpdateDate = DateTimeOffset.UtcNow;
        
        context.UserDevices.Update(device);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}