using OtpNet;

namespace eShop.Auth.Api.Security.TwoFactor.Recovery;

public class RecoveryCodeFactory : IRecoveryCodeFactory
{
    public IEnumerable<string> Create(int amount, int length)
    {
        for (var i = 0; i < amount; i++)
        {
            var keyBytes = KeyGeneration.GenerateRandomKey(length);
            var keyString = Base32Encoding.ToString(keyBytes);
            yield return keyString[..length]!;
        }
    }
}