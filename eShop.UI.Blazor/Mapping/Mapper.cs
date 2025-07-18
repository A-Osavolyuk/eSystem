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

    public static SendTwoFactorTokenRequest Map(SendTwoFactorTokenModel source)
    {
        return new SendTwoFactorTokenRequest()
        {
            Provider = source.Provider
        };
    }

    public static LockoutModel Map(LockoutStateDto source)
    {
        return new LockoutModel()
        {
            Id = source.Id,
            Description = source.Description,
            Enabled = source.Enabled,
            EndDate = source.EndDate,
            Permanent = source.Permanent,
            Reason = Map(source.Reason),
            StartDate = source.StartDate,
        };
    }

    public static LockoutReasonModel Map(LockoutReasonDto? source)
    {
        if (source is null) return new();
        
        return new LockoutReasonModel()
        {
            Description = source.Description,
            Id = source.Id,
            Name = source.Name,
            Period = source.Period,
            Type = source.Type,
            Code = source.Code,
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
            TwoFactorEnabled = source.TwoFactorEnabled,
        };
    }

    public static ChangeUserNameRequest Map(ChangeUserNameModel source)
    {
        return new()
        {
            UserId = source.Id,
            UserName = source.UserName
        };
    }

    public static ChangePersonalDataRequest Map(ChangePersonalDataModel source)
    {
        return new()
        {
            UserId = source.Id,
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
            UserId = source.Id,
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
            UserId = source.Id,
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
            UserId = source.Id
        };
    }
}