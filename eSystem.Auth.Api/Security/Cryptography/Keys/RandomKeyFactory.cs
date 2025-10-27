using eSystem.Core.Security.Cryptography.Keys;
using OtpNet;

namespace eSystem.Auth.Api.Security.Cryptography.Keys;

public class RandomKeyFactory : IKeyFactory
{
    public string Create(int length)
    {
        var keyBytes = KeyGeneration.GenerateRandomKey(length);
        var keyString = Base32Encoding.ToString(keyBytes);
        return keyString[..length]!;
    }
}