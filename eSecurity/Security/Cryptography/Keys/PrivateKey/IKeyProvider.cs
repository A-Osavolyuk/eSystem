using System.Security.Cryptography;

namespace eSecurity.Security.Cryptography.Keys.PrivateKey;

public interface IKeyProvider
{
    public Task<RSA?> GetPublicKeyAsync();
    public Task<RSA?> GetPrivateKeyAsync();
}