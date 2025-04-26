using eShop.Domain.Requests.API.Account;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Admin;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Mapping;

public static class Mapper
{
    public static PersonalDataResponse ToPersonalDataResponse(PersonalData entity)
    {
        return new PersonalDataResponse()
        {
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Gender = entity.Gender,
            DateOfBirth = entity.BirthDate
        };
    }

    public static PersonalDataEntity ToPersonalDataEntity(ChangePersonalDataRequest request)
    {
        return new PersonalDataEntity()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth!.Value
        };
    }

    public static PersonalDataEntity ToPersonalDataEntity(SetPersonalDataRequest request)
    {
        return new PersonalDataEntity()
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            DateOfBirth = request.BirthDate,
        };
    }

    public static PersonalData ToPersonalData(PersonalDataEntity data)
    {
        return new PersonalData()
        {
            FirstName = data.FirstName,
            LastName = data.LastName,
            Gender = data.Gender,
            BirthDate = data.DateOfBirth,
        };
    }

    public static UserEntity ToAppUser(RegistrationRequest request)
    {
        return new UserEntity()
        {
            Email = request.Email,
            UserName = request.Email,
            NormalizedEmail = request.Email.ToUpper(),
            NormalizedUserName = request.Email.ToUpper(),
        };
    }

    public static AccountData ToAccountData(UserEntity userEntity)
    {
        return new AccountData()
        {
            Id = userEntity.Id,
            Email = userEntity.Email!,
            UserName = userEntity.UserName!,
            PhoneNumber = userEntity.PhoneNumber!,
            EmailConfirmed = userEntity.EmailConfirmed,
            LockoutEnabled = userEntity.LockoutEnabled,
            LockoutEnd = userEntity.LockoutEnd,
            PhoneNumberConfirmed = userEntity.PhoneNumberConfirmed,
        };
    }

    public static LockoutStatusResponse ToUserLockoutStatusResponse(LockoutStatus status)
    {
        return new LockoutStatusResponse()
        {
            LockoutEnd = status.LockoutEnd,
            LockoutEnabled = status.LockoutEnabled
        };
    }
}