using eSecurity.Idp.Data.Entities;
using eSecurity.WebAuthN;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Credentials.PublicKey;

public interface ISoftwareKeyManager
{
    public ValueTask<List<SoftwareKeyEntity>> GetAllAsync(UserEntity user, CancellationToken cancellationToken);
    public ValueTask<SoftwareKeyEntity?> FindByDeviceAsync(UserDeviceEntity device, CancellationToken cancellationToken);
    public ValueTask<SoftwareKeyEntity?> FindByCredentialIdAsync(string credentialId, CancellationToken cancellationToken);
    public ValueTask<SoftwareKeyEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    public ValueTask<Result> CreateAsync(SoftwareKeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(SoftwareKeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(SoftwareKeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(SoftwareKeyEntity softwareKey, PublicKeyCredential credential,
        string storedChallenge, CancellationToken cancellationToken);
    
    public ValueTask<bool> HasAsync(UserEntity user, CancellationToken cancellationToken);
}