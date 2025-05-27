namespace eShop.Auth.Api.Services;

[Injectable(typeof(IProfileManager), ServiceLifetime.Scoped)]
public sealed class ProfileManager(AuthDbContext context) : IProfileManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<PersonalDataEntity?> FindAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var entity = await context.PersonalData
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == userEntity.PersonalDataId, cancellationToken: cancellationToken);

        return entity;
    }

    public async ValueTask<Result> SetAsync(UserEntity userEntity, PersonalDataEntity personalData,
        CancellationToken cancellationToken = default)
    {
        userEntity.PersonalDataId = personalData.Id;
        await context.PersonalData.AddAsync(personalData, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(UserEntity userEntity, PersonalDataEntity personalData,
        CancellationToken cancellationToken = default)
    {
        var data = await context.PersonalData
            .FirstOrDefaultAsync(x => x.Id == userEntity.PersonalDataId, cancellationToken: cancellationToken);

        if (data is null)
        {
            return Results.NotFound("Cannot find personal data");
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

        return Result.Success();
    }

    public async ValueTask<Result> DeleteAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default)
    {
        var data = await context.PersonalData
            .FirstOrDefaultAsync(x => x.Id == userEntity.PersonalDataId, cancellationToken: cancellationToken);

        if (data is null)
        {
            return Results.NotFound("Cannot find personal data");
        }

        context.PersonalData.Remove(data);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}