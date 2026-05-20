using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.Subject.Pairwise;

public interface IPairwiseSubjectManager
{
    public ValueTask<PairwiseSubjectEntity?> FindAsync(UserEntity user, 
        ClientEntity client, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(PairwiseSubjectEntity subject, 
        CancellationToken cancellationToken = default);
}