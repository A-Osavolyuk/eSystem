using eSystem.Auth.Api.Data.Entities;
using eSystem.Core.DTOs;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Security.Identity.Email;
using eSystem.Core.Security.Identity.PhoneNumber;

namespace eSystem.Auth.Api.Mapping;

public static class Mapper
{
    public static PersonalDataEntity Map(AddPersonalDataRequest source)
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

    public static RoleEntity Map(CreateRoleRequest source)
    {
        return new RoleEntity()
        {
            Id = Guid.CreateVersion7(),
            Name = source.Name,
            NormalizedName = source.Name.ToUpper(),
            CreateDate = DateTime.UtcNow,
            UpdateDate = null
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