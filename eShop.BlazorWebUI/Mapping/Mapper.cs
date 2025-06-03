using System.Globalization;
using eShop.BlazorWebUI.Models;
using eShop.Domain.DTOs;
using eShop.Domain.Models;
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
    
    public static ProfileModel Map(UserModel source)
    {
        return new ProfileModel()
        {
            UserName = source.UserName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber ?? "-",
            HasPersonalData = source.HasPersonalData,
            BirthDate = source.PersonalData?.BirthDate?.ToString("D", new CultureInfo("en-GB")) ?? string.Empty,
            FullName = source.PersonalData?.FullName ?? string.Empty,
            Gender = source.PersonalData?.Gender ?? string.Empty,
        };
    }

    public static LockoutModel Map(LockoutStateDto source)
    {
        return new LockoutModel()
        {
            Description = source.Description,
            Enabled = source.Enabled,
            EndDate = source.EndDate,
            Permanent = source.Permanent,
            Reason = source.Reason,
            StartDate = source.StartDate
        };
    }
}