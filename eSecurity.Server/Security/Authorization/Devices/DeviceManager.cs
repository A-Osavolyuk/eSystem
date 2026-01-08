using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.Devices;

public class DeviceManager(AuthDbContext context) : IDeviceManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<UserDeviceEntity>> GetAllAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        return await _context.UserDevices
            .Where(x => x.UserId == user.Id)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<UserDeviceEntity?> FindAsync(UserEntity user, string userAgent, string ipAddress,
        CancellationToken cancellationToken)
    {
        return await _context.UserDevices.FirstOrDefaultAsync(
            x => x.UserId == user.Id && 
                 x.UserAgent == userAgent && 
                 x.IpAddress == ipAddress, cancellationToken);
    }

    public async ValueTask<UserDeviceEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.UserDevices.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
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

        _context.UserDevices.Update(device);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> BlockAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        device.IsBlocked = true;
        device.BlockedDate = DateTimeOffset.UtcNow;

        _context.UserDevices.Update(device);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UnblockAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        device.IsBlocked = false;
        device.BlockedDate = null;

        _context.UserDevices.Update(device);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}