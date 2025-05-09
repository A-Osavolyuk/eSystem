using OtpNet;

namespace eShop.Auth.Api.Utilities;

public static class SecurityHandler
{
    public static QrCode GenerateQrCode(string email, string secret, string issuer)
    {

        var otpUri = new OtpUri(OtpType.Totp, secret, email, issuer);
        var url = otpUri.ToString();
        var qrCode = new QrCode() { Url = url };
        
        return qrCode;
    }
}
