using eShop.Domain.DTOs;

namespace eShop.Auth.Api.Security.Authentication.TwoFactor.Authenticator;

public interface IQrCodeFactory
{
    public QrCode Create(string secret, string email, string issuer);
}