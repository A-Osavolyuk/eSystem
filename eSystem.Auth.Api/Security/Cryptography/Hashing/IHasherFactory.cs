namespace eSystem.Auth.Api.Security.Cryptography.Hashing;

public interface IHasherFactory
{
    public Hasher Create(HashAlgorithm algorithm);
}