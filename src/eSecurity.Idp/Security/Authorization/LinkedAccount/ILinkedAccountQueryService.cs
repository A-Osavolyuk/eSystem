using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authorization.LinkedAccount;

public interface ILinkedAccountQueryService
{
    ValueTask<List<UserLinkedAccountEntity>> ListByUserAsync(Guid userId, 
        CancellationToken cancellationToken = default);

    ValueTask<UserLinkedAccountEntity?> GetByTypeAsync(Guid userId, LinkedAccountType type,
        CancellationToken cancellationToken = default);
    
    ValueTask<UserLinkedAccountEntity?> GetByIdAsync(Guid linkedAccountId, 
        CancellationToken cancellationToken = default);

    ValueTask<bool> ExistsAsync(Guid userId, LinkedAccountType type, CancellationToken cancellationToken = default);

    ValueTask<bool> HasAnyAsync(Guid userId, CancellationToken cancellationToken = default);
}