namespace eShop.Auth.Api.Security.Cryptography;

public interface IKeyFactory
{
    public string Create(uint length);
}