using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Identity.Privacy;

public sealed class PersonalDataManager(AuthDbContext context) : IPersonalDataManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<Result> CreateAsync(PersonalDataEntity personalData, 
        CancellationToken cancellationToken = default)
    {
        await _context.PersonalData.AddAsync(personalData, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> UpdateAsync(PersonalDataEntity personalData, 
        CancellationToken cancellationToken = default)
    {
        _context.PersonalData.Update(personalData);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> DeleteAsync(PersonalDataEntity personalData, 
        CancellationToken cancellationToken = default)
    {
        _context.PersonalData.Remove(personalData);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}