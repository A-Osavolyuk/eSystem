using eSecurity.Core.Security.Identity;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Identity.Email;

public interface IEmailQueryService
{
    ValueTask<List<UserEmailEntity>> ListByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    
    ValueTask<List<UserEmailEntity>> ListByTypeAsync(Guid userId, EmailType emailType,
        CancellationToken cancellationToken = default);
    
    ValueTask<UserEmailEntity?> GetByEmailAsync(Guid userId, string email,
        CancellationToken cancellationToken = default);
    
    ValueTask<UserEmailEntity?> GetByTypeAsync(Guid userId, EmailType type,
        CancellationToken cancellationToken = default);

    ValueTask<UserEmailEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    ValueTask<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);
}