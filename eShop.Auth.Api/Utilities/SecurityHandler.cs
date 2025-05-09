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
    
    public static string GenerateToken()
    {
        var randomCode = new Random().Next(0, 999999).ToString();
        var token = randomCode.PadLeft(6, '0');
        return token;
    }

    public static QrCode GenerateQrCode(string email, string secret, string issuer)
    {

        var otpUri = new OtpUri(OtpType.Totp, secret, email, issuer);
        var url = otpUri.ToString();
        var qrCode = new QrCode() { Url = url };
        
        return qrCode;
    }

    public static bool VerifyAuthenticatorToken(string secret, string token)
    {
        var secretBytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(secretBytes);
        
        return totp.VerifyTotp(token, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}
