namespace eSecurity.Server.Security.Cryptography.Hashing;

public interface IHasherFactory
{
    public Hasher Create(HashAlgorithm algorithm);
}