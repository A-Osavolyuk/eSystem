using eSecurity.Core.Common.DTOs;

namespace eSecurity.Server.Security.Authentication.TwoFactor.Authenticator;

public interface IQrCodeFactory
{
    public QrCode Create(string secret, string email, string issuer);
}