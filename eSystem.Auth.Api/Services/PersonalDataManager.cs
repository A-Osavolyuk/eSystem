namespace eSystem.Auth.Api.Services;

[Injectable(typeof(IPersonalDataManager), ServiceLifetime.Scoped)]
public sealed class PersonalDataManager(AuthDbContext context) : IPersonalDataManager
{
    private readonly AuthDbContext context = context;

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