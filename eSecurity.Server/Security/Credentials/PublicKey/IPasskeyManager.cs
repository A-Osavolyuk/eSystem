using eSecurity.Core.Security.Credentials.PublicKey;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Credentials.PublicKey;

public interface IPasskeyManager
{
    public ValueTask<List<PasskeyEntity>> GetAllAsync(UserDeviceEntity device, CancellationToken cancellationToken);
    public ValueTask<PasskeyEntity?> FindByCredentialIdAsync(string credentialId, CancellationToken cancellationToken);
    public ValueTask<PasskeyEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    public ValueTask<Result> CreateAsync(PasskeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(PasskeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(PasskeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(PasskeyEntity passkey, PublicKeyCredential credential,
        string storedChallenge, CancellationToken cancellationToken);
}