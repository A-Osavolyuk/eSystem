using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.Privacy;

public interface IPersonalDataManager
{
    public ValueTask<PersonalDataEntity?> GetAsync(Guid userId, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(PersonalDataEntity personalData,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> UpdateAsync(PersonalDataEntity personalData,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> DeleteAsync(PersonalDataEntity personalData,
        CancellationToken cancellationToken = default);
}