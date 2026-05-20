using eSecurity.Core.DTOs;

namespace eSecurity.Idp.Security.Authentication.TwoFactor.Authenticator;

public interface IQrCodeFactory
{
    public QrCode Create(string secret, string email, string issuer);
}