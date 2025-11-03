namespace eSecurity.Security.Cryptography.Hashing;

public interface IHasherFactory
{
    public Hasher Create(HashAlgorithm algorithm);
}