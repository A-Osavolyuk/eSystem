using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.Subject;

public interface ISubjectManager
{
    public ValueTask<Result> CreatePublicSubjectAsync(PublicSubjectEntity subject, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreatePairwiseSubjectAsync(PairwiseSubjectEntity subject, 
        CancellationToken cancellationToken = default);
}