namespace eShop.Auth.Api.Services;

internal sealed class ProfileManager(AuthDbContext context) : IProfileManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<PersonalData?> FindAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var data = await context.PersonalData
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userEntity.PersonalDataId, cancellationToken: cancellationToken);

        if (data is null)
        {
            return null;
        }

        var response = Mapper.Map(data);
        return response;
    }

    public async ValueTask<IdentityResult> SetAsync(UserEntity userEntity, PersonalDataEntity personalData,
        CancellationToken cancellationToken = default)
    {
        userEntity.PersonalDataId = personalData.Id;
        await context.PersonalData.AddAsync(personalData, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public async ValueTask<IdentityResult> UpdateAsync(UserEntity userEntity, PersonalDataEntity personalData,
        CancellationToken cancellationToken = default)
    {
        var data = await context.PersonalData
            .FirstOrDefaultAsync(x => x.Id == userEntity.PersonalDataId, cancellationToken: cancellationToken);

        if (data is null)
        {
            return IdentityResult.Failed(new IdentityError()
            {
                Code = "404",
                Description = "User didn't set personal data yet"
            });
        }

        var newData = new PersonalDataEntity()
        {
            Id = data.Id,
            Gender = personalData.Gender,
            FirstName = personalData.FirstName,
            LastName = personalData.LastName,
            DateOfBirth = personalData.DateOfBirth
        };

        context.PersonalData.Update(newData);
        await context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public async ValueTask<IdentityResult> DeleteAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default)
    {
        var data = await context.PersonalData
            .FirstOrDefaultAsync(x => x.Id == userEntity.PersonalDataId, cancellationToken: cancellationToken);

        if (data is null)
        {
            return IdentityResult.Failed(
                new IdentityError() { Code = "404", Description = "Cannot find personal data" });
        }

        context.PersonalData.Remove(data);
        await context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }
}