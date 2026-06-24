using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Identity.User;

public interface ICurrentUserAccessor
{
    ValueTask<UserEntity?> GetCurrentOrDefaultAsync(CancellationToken cancellationToken = default);
    
    ValueTask<UserEntity> GetRequiredCurrentAsync(CancellationToken cancellationToken = default);
}