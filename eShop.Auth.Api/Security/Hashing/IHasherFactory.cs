namespace eShop.Auth.Api.Security.Hashing;

public interface IHasherFactory
{
    public Hasher Create(HashAlgorithm algorithm);
}