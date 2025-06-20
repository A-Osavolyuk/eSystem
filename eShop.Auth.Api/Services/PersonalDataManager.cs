namespace eShop.Auth.Api.Services;

[Injectable(typeof(IPersonalDataManager), ServiceLifetime.Scoped)]
public sealed class PersonalDataManager(AuthDbContext context) : IPersonalDataManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<PersonalDataEntity?> FindAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var entity = await context.PersonalData
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userEntity.Id, cancellationToken: cancellationToken);

        return entity;
    }

    public async ValueTask<Result> CreateAsync(PersonalDataEntity personalData, CancellationToken cancellationToken = default)
    {
        await context.PersonalData.AddAsync(personalData, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(PersonalDataEntity personalData, CancellationToken cancellationToken = default)
    {
        context.PersonalData.Update(personalData);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> DeleteAsync(PersonalDataEntity personalData, CancellationToken cancellationToken = default)
    {
        context.PersonalData.Remove(personalData);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}