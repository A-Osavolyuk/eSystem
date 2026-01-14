using eSecurity.Core.Security.Credentials.PublicKey;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Credentials.PublicKey;

public interface IPasskeyManager
{
    public ValueTask<List<PasskeyEntity>> GetAllAsync(UserEntity user, CancellationToken cancellationToken);
    public ValueTask<PasskeyEntity?> FindByDeviceAsync(UserDeviceEntity device, CancellationToken cancellationToken);
    public ValueTask<PasskeyEntity?> FindByCredentialIdAsync(string credentialId, CancellationToken cancellationToken);
    public ValueTask<PasskeyEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    public ValueTask<Result> CreateAsync(PasskeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(PasskeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(PasskeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(PasskeyEntity passkey, PublicKeyCredential credential,
        string storedChallenge, CancellationToken cancellationToken);
    
    public ValueTask<bool> HasAsync(UserEntity user, CancellationToken cancellationToken);
}