namespace eShop.Auth.Api.Security.Cryptography.Keys;

public interface IKeyFactory
{
    public string Create(uint length);
}