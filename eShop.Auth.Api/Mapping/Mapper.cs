using eShop.Domain.DTOs;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Mapping;

public static class Mapper
{
    public static PersonalDataEntity Map(ChangePersonalDataRequest source)
    {
        return new PersonalDataEntity()
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            Gender = source.Gender,
            DateOfBirth = source.BirthDate!.Value
        };
    }

    public static PersonalDataEntity Map(AddPersonalDataRequest source)
    {
        return new PersonalDataEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = source.Id,
            FirstName = source.FirstName,
            LastName = source.LastName,
            Gender = source.Gender,
            DateOfBirth = source.BirthDate,
            CreateDate = DateTimeOffset.UtcNow
        };
    }

    public static PersonalDataDto Map(PersonalDataEntity source)
    {
        return new PersonalDataDto()
        {
            FirstName = source.FirstName,
            LastName = source.LastName,
            Gender = source.Gender,
            BirthDate = source.DateOfBirth,
            UpdateDate = source.UpdateDate
        };
    }

    public static UserEntity Map(RegistrationRequest source)
    {
        return new UserEntity()
        {
            Id = Guid.CreateVersion7(),
            Email = source.Email,
            UserName = source.Email,
            NormalizedEmail = source.Email.ToUpper(),
            NormalizedUserName = source.Email.ToUpper(),
        };
    }

    public static UserDto Map(UserEntity source)
    {
        return new()
        {
            Id = source.Id,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Username = source.UserName,
            TwoFactorEnabled = source.TwoFactorEnabled,
            EmailChangeDate = source.EmailChangeDate,
            PhoneNumberChangeDate = source.PhoneNumberChangeDate,
            UserNameChangeDate = source.UserNameChangeDate,
            PasswordChangeDate = source.PasswordChangeDate
        };
    }

    public static LockoutStatusResponse Map(LockoutStatus source)
    {
        return new LockoutStatusResponse()
        {
            LockoutEnd = source.LockoutEnd,
            LockoutEnabled = source.LockoutEnabled
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
    
    public static PermissionDto Map(PermissionEntity source)
    {
        return new PermissionDto()
        {
            Id = source.Id,
            Name = source.Name
        };
    }

    public static ProviderDto Map(ProviderEntity source)
    {
        return new ProviderDto()
        {
            Id = source.Id,
            Name = source.Name
        };
    }

    public static LockoutStateDto Map(LockoutStateEntity source)
    {
        return new()
        {
            Enabled = source.Enabled,
            Permanent = source.Permanent,
            Description = source.Description,
            Reason = source.Reason,
            EndDate = source.EndDate,
            StartDate = source.StartDate
        };
    }
}