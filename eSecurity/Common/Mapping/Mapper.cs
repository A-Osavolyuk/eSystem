using eSecurity.Common.Models;
using eSecurity.Data.Entities;
using eSecurity.Features.Users.Commands;
using eSystem.Core.DTOs;
using eSystem.Core.Security.Identity.Email;
using eSystem.Core.Security.Identity.PhoneNumber;

namespace eSecurity.Common.Mapping;

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
            Type = source.Type,
            StartDate = source.StartDate,
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

    public static ChangePasswordCommand Map(ChangePasswordModel source)
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
    
        public static PersonalDataEntity Map(AddPersonalDataCommand source)
    {
        return new PersonalDataEntity()
        {
            Id = Guid.CreateVersion7(),
            FirstName = source.FirstName,
            LastName = source.LastName,
            MiddleName = source.MiddleName,
            Gender = source.Gender,
            BirthDate = source.BirthDate,
            CreateDate = DateTimeOffset.UtcNow
        };
    }

    public static UserPersonalDto Map(PersonalDataEntity source)
    {
        return new UserPersonalDto()
        {
            UserId = source.User.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            MiddleName = source.MiddleName,
            Gender = source.Gender,
            BirthDate = source.BirthDate,
            UpdateDate = source.UpdateDate
        };
    }

    public static UserDto Map(UserEntity source)
    {
        var primaryEmail = source.GetEmail(EmailType.Primary);
        var primaryPhoneNumber = source.GetPhoneNumber(PhoneNumberType.Primary);
        
        return new()
        {
            Id = source.Id,
            Email = primaryEmail?.Email,
            EmailConfirmed = primaryEmail?.IsVerified,
            EmailChangeDate = primaryEmail?.UpdateDate,
            EmailConfirmationDate = primaryEmail?.VerifiedDate,
            PhoneNumber = primaryPhoneNumber?.PhoneNumber,
            PhoneNumberConfirmed = primaryPhoneNumber?.IsVerified,
            PhoneNumberChangeDate = primaryPhoneNumber?.UpdateDate,
            PhoneNumberConfirmationDate = primaryPhoneNumber?.VerifiedDate,
            Username = source.Username,
            UserNameChangeDate = source.UsernameChangeDate,
        };
    }

    public static UserDeviceDto Map(UserDeviceEntity source)
    {
        return new UserDeviceDto()
        {
            Id = source.Id,
            IpAddress = source.IpAddress,
            OS = source.OS,
            Browser = source.Browser,
            Device = source.Device,
            BlockedDate = source.BlockedDate,
            FirstSeen = source.FirstSeen,
            LastSeen = source.LastSeen,
            UserAgent = source.UserAgent,
            Location = source.Location,
            IsBlocked = source.IsBlocked,
            IsTrusted = source.IsTrusted
        };
    }

    public static RoleDto Map(RoleEntity source)
    {
        return new RoleDto()
        {
            Id = source.Id,
            Name = source.Name,
            NormalizedName = source.NormalizedName
        };
    }

    public static LockoutStateDto Map(UserLockoutStateEntity source)
    {
        return new()
        {
            Id = source.Id,
            Type = source.Type,
            Enabled = source.Enabled,
            Permanent = source.Permanent,
            Description = source.Description,
            EndDate = source.EndDate,
            StartDate = source.StartDate,
        };
    }

    public static PermissionDto Map (PermissionEntity source)
    {
        return new()
        {
            Id = source.Id,
            Name = source.Name
        };
    }
}