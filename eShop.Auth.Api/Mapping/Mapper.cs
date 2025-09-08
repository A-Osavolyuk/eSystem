using eShop.Domain.DTOs;
using eShop.Domain.Requests.API.Auth;

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

    public static UserPasskeyDto Map(UserPasskeyEntity source)
    {
        return new()
        {
            Id = source.Id,
            DisplayName = source.DisplayName,
            CreateDate = source.CreateDate,
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

    public static UserEntity Map(RegistrationRequest source)
    {
        return new UserEntity()
        {
            Id = Guid.CreateVersion7(),
            Username = source.UserName,
            NormalizedUsername = source.UserName.ToUpper(),
        };
    }

    public static UserDto Map(UserEntity source)
    {
        return new()
        {
            Id = source.Id,
            Email = source.Emails.FirstOrDefault(x => x.IsPrimary)?.Email,
            EmailConfirmed = source.Emails.FirstOrDefault(x => x.IsPrimary)?.IsVerified,
            EmailChangeDate = source.Emails.FirstOrDefault(x => x.IsPrimary)?.UpdateDate,
            EmailConfirmationDate = source.Emails.FirstOrDefault(x => x.IsPrimary)?.VerifiedDate,
            PhoneNumber = source.PhoneNumbers.FirstOrDefault(x => x.IsPrimary)?.PhoneNumber,
            PhoneNumberConfirmed = source.PhoneNumbers.FirstOrDefault(x => x.IsPrimary)?.IsVerified,
            PhoneNumberChangeDate = source.PhoneNumbers.FirstOrDefault(x => x.IsPrimary)?.UpdateDate,
            PhoneNumberConfirmationDate = source.PhoneNumbers.FirstOrDefault(x => x.IsPrimary)?.VerifiedDate,
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

    public static UserProviderDto Map(UserTwoFactorProviderEntity source)
    {
        return new UserProviderDto()
        {
            Id = source.TwoFactorProvider.Id,
            Name = source.TwoFactorProvider.Name,
            Subscribed = source.Subscribed,
            UpdateDate = source.UpdateDate,
        };
    }
    
    public static ProviderDto Map(TwoFactorProviderEntity source)
    {
        return new ProviderDto()
        {
            Id = source.Id,
            Name = source.Name,
        };
    }

    public static LockoutStateDto Map(LockoutStateEntity source)
    {
        return new()
        {
            Id = source.Id,
            Reason = Map(source.Reason),
            Enabled = source.Enabled,
            Permanent = source.Permanent,
            Description = source.Description,
            EndDate = source.EndDate,
            StartDate = source.StartDate,
        };
    }

    public static LockoutReasonDto Map(LockoutReasonEntity? source)
    {
        if (source is null) return new();
        
        return new()
        {
            Id = source.Id,
            Code = source.Code,
            Description = source.Description,
            Name = source.Name,
            Period = source.Period,
            Type = source.Type
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