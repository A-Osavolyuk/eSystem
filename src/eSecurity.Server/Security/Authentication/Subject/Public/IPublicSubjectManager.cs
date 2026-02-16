using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Subject.Public;

public interface IPublicSubjectManager
{
    public ValueTask<PublicSubjectEntity?> FindAsync(UserEntity user, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> CreateAsync(PublicSubjectEntity subject, 
        CancellationToken cancellationToken = default);
}