using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Identity.Privacy;

public sealed class PersonalDataManager(AuthDbContext context) : IPersonalDataManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<PersonalDataEntity?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.PersonalData.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(PersonalDataEntity personalData, 
        CancellationToken cancellationToken = default)
    {
        await _context.PersonalData.AddAsync(personalData, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> UpdateAsync(PersonalDataEntity personalData, 
        CancellationToken cancellationToken = default)
    {
        _context.PersonalData.Update(personalData);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> DeleteAsync(PersonalDataEntity personalData, 
        CancellationToken cancellationToken = default)
    {
        _context.PersonalData.Remove(personalData);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Success(SuccessCodes.Ok);
    }
}