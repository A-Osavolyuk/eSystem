namespace eShop.Auth.Api.Security.Cryptography;

public interface ICodeFactory
{
    public string Create(uint length = 6);
}