using eShop.BlazorWebUI.Models;
using eShop.Domain.Requests.API.Auth;

namespace eShop.BlazorWebUI.Mapping;

public static class Mapper
{
    public static LoginRequest Map(LoginModel source)
    {
        return new LoginRequest()
        {
            Email = source.Email,
            Password = source.Password
        };
    }
    
    public static TwoFactorLoginRequest Map(TwoFactorLoginModel source)
    {
        return new TwoFactorLoginRequest()
        {
            Email = source.Email,
            Token = source.Code
        };
    }

    public static RegistrationRequest Map(RegisterModel source)
    {
        return new RegistrationRequest()
        {
            Email = source.Email,
            Password = source.Password,
            ConfirmPassword = source.ConfirmPassword
        };
    }

    public static VerifyEmailRequest Map(VerifyEmailModel source)
    {
        return new VerifyEmailRequest()
        {
            Email = source.Email,
            Code = source.Code
        };
    }

    public static SendTwoFactorTokenRequest Map(SendTwoFactorTokenModel source)
    {
        return new SendTwoFactorTokenRequest()
        {
            Email = source.Email,
            Provider = source.Provider
        };
    }
}