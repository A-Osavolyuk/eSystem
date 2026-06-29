using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Identity.Privacy;

public interface IPersonalDataQueryService
{
    ValueTask<PersonalDataEntity?> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}