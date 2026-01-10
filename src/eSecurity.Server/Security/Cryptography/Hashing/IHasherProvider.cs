namespace eSecurity.Server.Security.Cryptography.Hashing;

public interface IHasherProvider
{
    public IHasher GetHasher(HashAlgorithm algorithm);
}