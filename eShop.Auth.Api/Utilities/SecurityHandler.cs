using OtpNet;

namespace eShop.Auth.Api.Utilities;

public static class SecurityHandler
{
    public static string GenerateSecret()
    {
        var secretKey = KeyGeneration.GenerateRandomKey(20);
        var base32Secret = Base32Encoding.ToString(secretKey);

        return base32Secret;
    }
}

