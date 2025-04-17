namespace eShop.Auth.Api.Services;

internal sealed class ProfileManager(AuthDbContext context) : IProfileManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<PersonalData?> FindAsync(AppUser user, CancellationToken cancellationToken = default)
    {
        var data = await context.PersonalData
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken: cancellationToken);

        if (data is null)
        {
            return null;
        }

        var response = Mapper.ToPersonalData(data);
        return response;
    }

    public async ValueTask<IdentityResult> SetAsync(AppUser user, PersonalDataEntity personalData,
        CancellationToken cancellationToken = default)
    {
        personalData = personalData with { UserId = user.Id };
        await context.PersonalData.AddAsync(personalData, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public async ValueTask<IdentityResult> UpdateAsync(AppUser user, PersonalDataEntity personalData,
        CancellationToken cancellationToken = default)
    {
        var data = await context.PersonalData
            .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken: cancellationToken);

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
            UserId = user.Id,
            Gender = personalData.Gender,
            FirstName = personalData.FirstName,
            LastName = personalData.LastName,
            DateOfBirth = personalData.DateOfBirth
        };

        context.PersonalData.Update(newData);
        await context.SaveChangesAsync(cancellationToken);

        return IdentityResult.Success;
    }

    public async ValueTask<IdentityResult> DeleteAsync(AppUser user,
        CancellationToken cancellationToken = default)
    {
        var data = await context.PersonalData
            .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken: cancellationToken);

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