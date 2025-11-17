using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.Devices;

public class DeviceManager(AuthDbContext context) : IDeviceManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<UserDeviceEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var device = await _context.UserDevices.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return device;
    }

    public async ValueTask<Result> CreateAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        if (device.Device == "Other")
        {
            device.Device = "Desktop";
        }
        
        await _context.UserDevices.AddAsync(device, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Ok();
    }

    public async ValueTask<Result> TrustAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        device.IsTrusted = true;
        device.UpdateDate = DateTimeOffset.UtcNow;
        
        _context.UserDevices.Update(device);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> BlockAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        device.IsBlocked = true;
        device.BlockedDate = DateTimeOffset.UtcNow;
        device.UpdateDate = DateTimeOffset.UtcNow;
        
        _context.UserDevices.Update(device);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UnblockAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        device.IsBlocked = false;
        device.BlockedDate = null;
        device.UpdateDate = DateTimeOffset.UtcNow;
        
        _context.UserDevices.Update(device);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}