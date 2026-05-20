namespace eSecurity.Idp.Security.Cryptography.Hashing;

public interface IHasherProvider
{
    public IHasher GetHasher(HashAlgorithm algorithm);
}