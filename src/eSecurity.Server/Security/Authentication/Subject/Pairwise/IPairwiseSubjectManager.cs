using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Subject.Pairwise;

public interface IPairwiseSubjectManager
{
    public ValueTask<PairwiseSubjectEntity?> FindAsync(UserEntity user, 
        ClientEntity client, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(PairwiseSubjectEntity subject, 
        CancellationToken cancellationToken = default);
}