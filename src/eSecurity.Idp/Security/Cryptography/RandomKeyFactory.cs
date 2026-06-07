using OtpNet;

namespace eSecurity.Idp.Security.Cryptography;

public static class RandomKeyFactory
{
    public static string Create(int length)
    {
        var keyBytes = KeyGeneration.GenerateRandomKey(length);
        var keyString = Base32Encoding.ToString(keyBytes);
        return keyString[..length]!;
    }
}