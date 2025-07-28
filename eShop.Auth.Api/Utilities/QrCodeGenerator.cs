using OtpNet;

namespace eShop.Auth.Api.Utilities;

public static class QrCodeGenerator
{
    public static string Generate(string email, string secret, string issuer)
    {

        var otpUri = new OtpUri(OtpType.Totp, secret, email, issuer);
        var url = otpUri.ToString();
        
        return url;
    }
}