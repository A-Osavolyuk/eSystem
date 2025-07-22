using OtpNet;

namespace eShop.Auth.Api.Utilities;

public class PasswordGenerator
{
    public static string Generate(int length)
    {
        var key = KeyGeneration.GenerateRandomKey(length);
        var password = Base32Encoding.ToString(key);

        return password;
    }
}