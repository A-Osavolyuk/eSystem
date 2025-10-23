using eSystem.Domain.DTOs;

namespace eSystem.Auth.Api.Security.Authentication.TwoFactor.Authenticator;

public interface IQrCodeFactory
{
    public QrCode Create(string secret, string email, string issuer);
}