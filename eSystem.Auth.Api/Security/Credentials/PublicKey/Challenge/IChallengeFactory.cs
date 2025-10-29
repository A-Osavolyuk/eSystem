namespace eSystem.Auth.Api.Security.Credentials.PublicKey.Challenge;

public interface IChallengeFactory
{
    public string Create(uint length = 32);
}