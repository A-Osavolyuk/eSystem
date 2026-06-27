using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Credentials.PublicKey;

public interface ISoftwareKeyQueryService
{
    ValueTask<List<SoftwareKeyEntity>> ListByUserAsync(Guid userId, 
        CancellationToken cancellationToken = default);

    ValueTask<SoftwareKeyEntity?> GetByIdAsync(Guid keyId, 
        CancellationToken cancellationToken = default);
    
    ValueTask<SoftwareKeyEntity?> GetByCredentialIdAsync(byte[] credentialId, 
        CancellationToken cancellationToken = default);

    ValueTask<SoftwareKeyEntity?> GetByDeviceAsync(Guid deviceId, 
        CancellationToken cancellationToken = default);
}