using OtpNet;

namespace eSecurity.Server.Security.Authentication.TwoFactor.Authenticator;

public static class AuthenticatorUtils
{
    public static bool VerifyCode(string code, string secret)
    {
        var secretBytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(secretBytes);
        return totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);
    }
}