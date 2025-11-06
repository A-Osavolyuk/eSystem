using eSecurity.Common.DTOs;
using eSystem.Core.DTOs;

namespace eSecurity.Security.Authentication.TwoFactor.Authenticator;

public interface IQrCodeFactory
{
    public QrCode Create(string secret, string email, string issuer);
}