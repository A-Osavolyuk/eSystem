using eShop.Domain.DTOs;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Mapping;

public static class Mapper
{
    public static PersonalDataEntity Map(AddPersonalDataRequest source)
    {
        return new PersonalDataEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = source.UserId,
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
            NormalizedEmail = source.Email.ToUpper(),
            UserName = source.UserName,
            NormalizedUserName = source.UserName.ToUpper(),
        };
    }

    public static UserDto Map(UserEntity source)
    {
        return new()
        {
            Id = source.Id,
            Email = source.Email,
            RecoveryEmail = source.RecoveryEmail,
            PhoneNumber = source.PhoneNumber,
            Username = source.UserName,
            TwoFactorEnabled = source.Providers.Any(x => x.Subscribed),
            EmailChangeDate = source.EmailChangeDate,
            RecoveryEmailChangeDate = source.RecoveryEmailChangeDate,
            PhoneNumberChangeDate = source.PhoneNumberChangeDate,
            UserNameChangeDate = source.UserNameChangeDate,
            PasswordChangeDate = source.PasswordChangeDate,
            Providers = source.Providers.Select(Map).ToList(),
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

    public static UserProviderDto Map(UserProviderEntity source)
    {
        return new UserProviderDto()
        {
            Id = source.Provider.Id,
            Name = source.Provider.Name,
            Subscribed = source.Subscribed,
            UpdateDate = source.UpdateDate,
        };
    }
    
    public static ProviderDto Map(ProviderEntity source)
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
}