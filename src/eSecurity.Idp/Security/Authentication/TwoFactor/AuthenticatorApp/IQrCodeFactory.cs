using eSecurity.Core.DTOs;

namespace eSecurity.Idp.Security.Authentication.TwoFactor.AuthenticatorApp;

public interface IQrCodeFactory
{
    public QrCode Create(string secret, string email, string issuer);
}