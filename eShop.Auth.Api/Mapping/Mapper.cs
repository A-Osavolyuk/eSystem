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

    public static AppUser ToAppUser(RegistrationRequest request)
    {
        return new AppUser()
        {
            Email = request.Email,
            UserName = request.Email,
            NormalizedEmail = request.Email.ToUpper(),
            NormalizedUserName = request.Email.ToUpper(),
        };
    }

    public static AccountData ToAccountData(AppUser user)
    {
        return new AccountData()
        {
            Id = Guid.Parse(user.Id),
            Email = user.Email!,
            UserName = user.UserName!,
            PhoneNumber = user.PhoneNumber!,
            EmailConfirmed = user.EmailConfirmed,
            LockoutEnabled = user.LockoutEnabled,
            LockoutEnd = user.LockoutEnd,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
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