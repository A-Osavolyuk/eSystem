using System.Globalization;
using eShop.BlazorWebUI.Models;
using eShop.Domain.DTOs;
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

    private static PersonalDataModel Map(PersonalDataDto source)
    {
        return new PersonalDataModel()
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            BirthDate = source.BirthDate,
            Gender = source.Gender,
            UpdateDate = source.UpdateDate
        };
    }

    public static ChangeEmailRequest Map(ChangeEmailModel source)
    {
        return new ChangeEmailRequest()
        {
            UserId = source.UserId,
            NewEmail = source.NewEmail
        };
    }

    public static UserModel Map(UserDto source)
    {
        return new UserModel()
        {
            Id = source.Id,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            UserName = source.Username,
            EmailChangeDate = source.EmailChangeDate,
            PasswordChangeDate = source.PasswordChangeDate,
            UserNameChangeDate = source.UserNameChangeDate,
            PhoneNumberChangeDate = source.PhoneNumberChangeDate,
            PersonalData = source.PersonalData is null ? null : Map(source.PersonalData),
        };
    }

    public static ChangeUserNameRequest Map(ChangeUserNameModel source)
    {
        return new()
        {
            Id = source.Id,
            UserName = source.UserName
        };
    }

    public static ChangePersonalDataRequest Map(ChangePersonalDataModel source)
    {
        return new()
        {
            Id = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            BirthDate = source.BirthDate,
            Gender = source.Gender,
        };
    }

    public static ChangePersonalDataModel Map(UserModel source)
    {
        return new()
        {
            Id = source.Id,
            Gender = source.PersonalData!.Gender,
            BirthDate = source.PersonalData!.BirthDate,
            FirstName = source.PersonalData!.FirstName,
            LastName = source.PersonalData!.LastName,
        };
    }

    public static AddPersonalDataRequest Map(AddPersonalDataModel source)
    {
        return new()
        {
            Id = source.Id,
            Gender = source.Gender,
            BirthDate = source.BirthDate!.Value,
            FirstName = source.FirstName,
            LastName = source.LastName,
        };
    }

    public static ChangePasswordRequest Map(ChangePasswordModel source)
    {
        return new()
        {
            Id = source.Id,
            CurrentPassword = source.CurrentPassword,
            NewPassword = source.NewPassword,
            ConfirmNewPassword = source.ConfirmNewPassword,
        };
    }

    public static ResetPasswordRequest Map(ResetPasswordModel source)
    {
        return new()
        {
            NewPassword = source.NewPassword,
            ConfirmNewPassword = source.ConfirmNewPassword,
            Id = source.Id
        };
    }
}