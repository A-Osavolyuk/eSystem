using eShop.Domain.DTOs;
using OtpNet;

namespace eShop.Auth.Api.Security.Authentication.TwoFactor.Authenticator;

public class QrCodeFactory : IQrCodeFactory
{
    public QrCode Create(string secret, string email, string issuer)
    {
        var otpUri = new OtpUri(OtpType.Totp, secret, email, issuer);
        var value = otpUri.ToString();
        
        return new QrCode()
        {
            Value = value,
            Secret = secret,
        };
    }
}