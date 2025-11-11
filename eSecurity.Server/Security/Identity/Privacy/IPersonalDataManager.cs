using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Identity.Privacy;

public interface IPersonalDataManager
{
    public ValueTask<Result> CreateAsync(PersonalDataEntity personalData, 
        CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(PersonalDataEntity personalData, 
        CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(PersonalDataEntity personalData, 
        CancellationToken cancellationToken = default);
}