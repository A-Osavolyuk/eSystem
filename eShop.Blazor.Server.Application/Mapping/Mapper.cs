using eShop.Blazor.Server.Domain.Models;
using eShop.Domain.Requests.Auth;

namespace eShop.Blazor.Server.Application.Mapping;

public static class Mapper
{
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

    public static ChangePersonalDataRequest Map(ChangePersonalDataModel source)
    {
        return new()
        {
            UserId = source.UserId,
            FirstName = source.FirstName,
            LastName = source.LastName,
            MiddleName = source.MiddleName,
            BirthDate = source.BirthDate,
            Gender = source.Gender,
        };
    }

    public static ChangePersonalDataModel Map(UserPersonalModel source)
    {
        return new()
        {
            UserId = source.UserId,
            Gender = source.Gender,
            BirthDate = source.BirthDate,
            FirstName = source.FirstName,
            LastName = source.LastName,
            MiddleName = source.MiddleName
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
            MiddleName = source.MiddleName
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

    public static UserPersonalModel Map(UserPersonalDto source)
    {
        return new()
        {
            UserId = source.UserId,
            FirstName = source.FirstName,
            LastName = source.LastName,
            MiddleName = source.MiddleName,
            BirthDate = source.BirthDate,
            Gender = source.Gender,
            UpdateDate = source.UpdateDate
        };
    }
}