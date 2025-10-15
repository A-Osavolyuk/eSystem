using eShop.Domain.DTOs;
using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Mapping;

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
        return new()
        {
            Id = source.Id,
            Email = source.Emails.FirstOrDefault(x => x.Type is EmailType.Primary)?.Email,
            EmailConfirmed = source.Emails.FirstOrDefault(x => x.Type is EmailType.Primary)?.IsVerified,
            EmailChangeDate = source.Emails.FirstOrDefault(x => x.Type is EmailType.Primary)?.UpdateDate,
            EmailConfirmationDate = source.Emails.FirstOrDefault(x => x.Type is EmailType.Primary)?.VerifiedDate,
            PhoneNumber = source.PhoneNumbers.FirstOrDefault(x => x.Type is PhoneNumberType.Primary)?.PhoneNumber,
            PhoneNumberConfirmed = source.PhoneNumbers.FirstOrDefault(x => x.Type is PhoneNumberType.Primary)?.IsVerified,
            PhoneNumberChangeDate = source.PhoneNumbers.FirstOrDefault(x => x.Type is PhoneNumberType.Primary)?.UpdateDate,
            PhoneNumberConfirmationDate = source.PhoneNumbers.FirstOrDefault(x => x.Type is PhoneNumberType.Primary)?.VerifiedDate,
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

    public static UserOAuthProviderDto Map(UserLinkedAccountEntity source)
    {
        return new UserOAuthProviderDto()
        {
            Id = source.Provider.Id,
            Name = source.Provider.Name,
            IsAllowed = source.Allowed,
            LinkedDate = source.CreateDate,
            DisallowedDate = source.UpdateDate,
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
            Name = source.Name ?? string.Empty,
            NormalizedName = source.NormalizedName ?? string.Empty
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