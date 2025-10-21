using OtpNet;

namespace eShop.Auth.Api.Security.Cryptography.Keys;

public class KeyFactory : IKeyFactory
{
    public string Create(uint length)
    {
        var keyBytes = KeyGeneration.GenerateRandomKey((int)length);
        var keyString = Base32Encoding.ToString(keyBytes);
        return keyString[..(int)length]!;
    }
}