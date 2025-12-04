namespace eSecurity.Server.Security.Cryptography.Hashing;

public interface IHasherFactory
{
    public IHasher CreateHasher(HashAlgorithm algorithm);
}