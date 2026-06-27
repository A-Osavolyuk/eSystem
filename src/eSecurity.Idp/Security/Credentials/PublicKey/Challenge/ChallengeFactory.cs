using OtpNet;

namespace eSecurity.Idp.Security.Credentials.PublicKey.Challenge;

public static class ChallengeFactory
{
    public static string Create(uint length = 32)
    {
        var challengeBytes = KeyGeneration.GenerateRandomKey((int)length);
        return Convert.ToBase64String(challengeBytes);
    }
}