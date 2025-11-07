using eSecurity.Data.Entities;
using eSecurity.Security.Credentials.PublicKey.Credentials;

namespace eSecurity.Security.Credentials.PublicKey;

public interface IPasskeyManager
{
    public ValueTask<PasskeyEntity?> FindByCredentialIdAsync(string credentialId, CancellationToken cancellationToken);
    public ValueTask<PasskeyEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    public ValueTask<Result> CreateAsync(PasskeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(PasskeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(PasskeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(PasskeyEntity passkey, PublicKeyCredential credential,
        string storedChallenge, CancellationToken cancellationToken);
}