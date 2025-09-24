using OtpNet;

namespace eShop.Auth.Api.Utilities;

public static class KeyGenerator
{
    public static string GenerateKey(uint length)
    {
        var keyBytes = KeyGeneration.GenerateRandomKey((int)length);
        var keyString = Base32Encoding.ToString(keyBytes);
        var key = keyString[0..(int)length]!;
        return key;
    }
}