namespace eShop.Auth.Api.Security.Credentials.PublicKey;

public interface IChallengeFactory
{
    public string Create(uint length = 32);
}