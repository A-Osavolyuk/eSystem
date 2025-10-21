using OtpNet;

namespace eShop.Auth.Api.Security.Credentials.PublicKey;

public class ChallengeFactory : IChallengeFactory
{
    public string Create(uint length = 32)
    {
        var challengeBytes = KeyGeneration.GenerateRandomKey((int)length);
        return Convert.ToBase64String(challengeBytes);
    }
}