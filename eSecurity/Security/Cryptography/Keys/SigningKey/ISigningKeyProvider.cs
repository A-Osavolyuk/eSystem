namespace eSecurity.Security.Cryptography.Keys.SigningKey;

public interface ISigningKeyProvider
{
    public Task<SigningKey> GetAsync(CancellationToken cancellationToken = default);
}