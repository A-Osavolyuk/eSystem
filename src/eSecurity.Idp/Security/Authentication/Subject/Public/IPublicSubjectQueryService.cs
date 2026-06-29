using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.Subject.Public;

public interface IPublicSubjectQueryService
{
    ValueTask<PublicSubjectEntity?> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
}