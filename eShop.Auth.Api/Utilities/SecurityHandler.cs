using OtpNet;

namespace eShop.Auth.Api.Utilities;

public static class SecurityHandler
{
    public static string GenerateSecret()
    {
        var randomGuid = Guid.NewGuid();
        var bytes = randomGuid.ToByteArray();
        var totp = new Totp(bytes);
        var secret = totp.ComputeTotp();

        return secret;
    }
}

