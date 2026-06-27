using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authorization.Devices;

public sealed class DeviceQueryService(AuthDbContext context) : IDeviceQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<UserDeviceEntity>> ListByUserAsync(Guid userId, 
        CancellationToken cancellationToken = default)
    {
        return await _context.UserDevices
            .Where(x => x.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<UserDeviceEntity?> GetByMetadataAsync(Guid userId, string userAgent, string ipAddress,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userAgent);
        ArgumentException.ThrowIfNullOrWhiteSpace(ipAddress);
        
        return await _context.UserDevices.FirstOrDefaultAsync(
            x => x.UserId == userId && x.UserAgent == userAgent && x.IpAddress == ipAddress, cancellationToken);
    }

    public async ValueTask<UserDeviceEntity?> GetByIdAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        return await _context.UserDevices.FirstOrDefaultAsync(x => x.Id == deviceId, cancellationToken);
    }
}