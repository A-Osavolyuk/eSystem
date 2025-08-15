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
            Login = source.Login,
            Password = source.Password
        };
    }
    
    public static TwoFactorLoginRequest Map(TwoFactorLoginModel source)
    {
        return new TwoFactorLoginRequest()
        {
            Code = source.Code
        };
    }

    public static RegistrationRequest Map(RegisterModel source)
    {
        return new RegistrationRequest()
        {
            UserName = source.UserName,
            Email = source.Email,
            Password = source.Password,
            ConfirmPassword = source.ConfirmPassword
        };
    }

    public static SendTwoFactorCodeRequest Map(SendTwoFactorTokenModel source)
    {
        return new SendTwoFactorCodeRequest()
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

    private static UserPersonalModel Map(PersonalDataDto source)
    {
        return new UserPersonalModel()
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
            UserId = source.Id,
            Email = source.Email,
            EmailConfirmed = source.EmailConfirmed,
            EmailConfirmationDate = source.EmailConfirmationDate,
            EmailChangeDate = source.EmailChangeDate,
            PhoneNumber = source.PhoneNumber,
            PhoneNumberConfirmed = source.PhoneNumberConfirmed,
            PhoneNumberConfirmationDate = source.PhoneNumberConfirmationDate,
            PhoneNumberChangeDate = source.PhoneNumberChangeDate,
            UserName = source.Username,
            UserNameChangeDate = source.UserNameChangeDate,
        };
    }

    public static UserSecurityModel Map(UserSecurityDto source)
    {
        return new UserSecurityModel()
        {
            UserId = source.UserId,
            RecoveryEmail = source.RecoveryEmail,
            RecoveryEmailConfirmed = source.RecoveryEmailConfirmed,
            RecoveryEmailConfirmationDate = source.RecoveryEmailConfirmationDate,
            RecoveryEmailChangeDate = source.RecoveryEmailChangeDate,
            HasPassword = source.HasPassword,
            PasswordChangeDate = source.PasswordChangeDate,
            TwoFactorEnabled = source.TwoFactorEnabled,
            Providers = source.Providers,
            OAuthProviders = source.OAuthProviders,
            Devices = source.Devices,
        };
    }

    public static ChangeUsernameRequest Map(ChangeUsernameModel source)
    {
        return new()
        {
            UserId = source.Id,
            Username = source.Username
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

    public static ChangePersonalDataModel Map(UserPersonalModel source)
    {
        return new()
        {
            Id = source.UserId,
            Gender = source!.Gender,
            BirthDate = source!.BirthDate,
            FirstName = source!.FirstName,
            LastName = source!.LastName,
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
            UserId = source.Id
        };
    }

    public static UserPersonalModel Map(UserPersonalDto source)
    {
        return new()
        {
            UserId = source.UserId,
            FirstName = source.FirstName,
            LastName = source.LastName,
            BirthDate = source.BirthDate,
            Gender = source.Gender
        };
    }
}