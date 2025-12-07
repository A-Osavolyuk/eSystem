using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Identity.Email;

public interface IEmailManager
{
    public ValueTask<List<UserEmailEntity>> GetAllAsync(UserEntity user, CancellationToken cancellationToken);
    
    public ValueTask<List<UserEmailEntity>> GetAllAsync(UserEntity user, EmailType type, 
        CancellationToken cancellationToken);
    
    public ValueTask<UserEmailEntity?> FindByTypeAsync(UserEntity user, EmailType type, 
        CancellationToken cancellationToken);
    
    public ValueTask<UserEmailEntity?> FindByEmailAsync(UserEntity user, string email, 
        CancellationToken cancellationToken);
}