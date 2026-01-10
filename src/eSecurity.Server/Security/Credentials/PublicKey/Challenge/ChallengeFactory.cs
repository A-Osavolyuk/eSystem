using OtpNet;

namespace eSecurity.Server.Security.Credentials.PublicKey.Challenge;

public class ChallengeFactory : IChallengeFactory
{
    public string Create(uint length = 32)
    {
        var challengeBytes = KeyGeneration.GenerateRandomKey((int)length);
        return Convert.ToBase64String(challengeBytes);
    }
}