using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Credentials.PublicKey;

public sealed class SoftwareKeyQueryService(AuthDbContext context) : ISoftwareKeyQueryService
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<List<SoftwareKeyEntity>> ListByUserAsync(Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SoftwareKeys
            .Where(key => _context.UserDevices
                .Any(device => device.UserId == userId && device.Id == key.DeviceId)
            )
            .ToListAsync(cancellationToken);
    }

    public async ValueTask<SoftwareKeyEntity?> GetByIdAsync(Guid keyId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SoftwareKeys.FirstOrDefaultAsync(key => key.Id == keyId, cancellationToken);
    }

    public async ValueTask<SoftwareKeyEntity?> GetByCredentialIdAsync(byte[] credentialId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(credentialId);
        
        return await _context.SoftwareKeys
            .FirstOrDefaultAsync(key => key.CredentialId == credentialId, cancellationToken);
    }

    public async ValueTask<SoftwareKeyEntity?> GetByDeviceAsync(Guid deviceId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SoftwareKeys
            .FirstOrDefaultAsync(key => key.DeviceId == deviceId, cancellationToken);
    }
}