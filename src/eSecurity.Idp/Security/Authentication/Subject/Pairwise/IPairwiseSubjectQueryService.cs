using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.Subject.Pairwise;

public interface IPairwiseSubjectQueryService
{
    ValueTask<PairwiseSubjectEntity?> GetByClientAsync(Guid userId, Guid clientId,
        CancellationToken cancellationToken = default);
}