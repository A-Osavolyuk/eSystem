using eShop.Domain.DTOs;
using eShop.Domain.Requests.API.Account;
using eShop.Domain.Requests.API.Admin;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Admin;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Mapping;

public static class Mapper
{
    public static PersonalDataResponse Map(PersonalDataDto entity)
    {
        return new PersonalDataResponse()
        {
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Gender = entity.Gender,
            DateOfBirth = entity.BirthDate
        };
    }

    public static PersonalDataEntity Map(ChangePersonalDataRequest request)
    {
        return new PersonalDataEntity()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth!.Value
        };
    }

    public static PersonalDataEntity Map(SetPersonalDataRequest request)
    {
        return new PersonalDataEntity()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            DateOfBirth = request.BirthDate,
        };
    }

    public static PersonalDataDto Map(PersonalDataEntity data)
    {
        return new PersonalDataDto()
        {
            FirstName = data.FirstName,
            LastName = data.LastName,
            Gender = data.Gender,
            BirthDate = data.DateOfBirth,
        };
    }

    public static UserEntity Map(RegistrationRequest request)
    {
        return new UserEntity()
        {
            Email = request.Email,
            UserName = request.Email,
            NormalizedEmail = request.Email.ToUpper(),
            NormalizedUserName = request.Email.ToUpper(),
        };
    }

    public static AccountDataDto Map(UserEntity userEntity)
    {
        return new AccountDataDto()
        {
            Id = userEntity.Id,
            Email = userEntity.Email!,
            UserName = userEntity.UserName!,
            PhoneNumber = userEntity.PhoneNumber!,
            EmailConfirmed = userEntity.EmailConfirmed,
            PhoneNumberConfirmed = userEntity.PhoneNumberConfirmed,
        };
    }

    public static LockoutStatusResponse Map(LockoutStatus status)
    {
        return new LockoutStatusResponse()
        {
            LockoutEnd = status.LockoutEnd,
            LockoutEnabled = status.LockoutEnabled
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
}