using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.Subject.Pairwise;

public interface IPairwiseSubjectCommandService
{
    ValueTask<Result> CreateAsync(PairwiseSubjectEntity entity, CancellationToken cancellationToken = default);
}