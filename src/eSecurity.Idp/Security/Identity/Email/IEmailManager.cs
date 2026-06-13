using eSecurity.Idp.Data.Entities;
using eSecurity.Core.Security.Identity;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.Email;

public interface IEmailManager
{
    public ValueTask<List<UserEmailEntity>> GetAllAsync(UserEntity user, CancellationToken cancellationToken);
    
    public ValueTask<List<UserEmailEntity>> GetAllAsync(UserEntity user, EmailType type, 
        CancellationToken cancellationToken);
    
    public ValueTask<UserEmailEntity?> FindByTypeAsync(UserEntity user, EmailType type, 
        CancellationToken cancellationToken);
    
    public ValueTask<UserEmailEntity?> FindByEmailAsync(UserEntity user, string email, 
        CancellationToken cancellationToken);
    
    public ValueTask<Result> SetAsync(UserEntity user, string email, 
        EmailType type, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> ChangeAsync(UserEntity user, string currentEmail, string newEmail,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> VerifyAsync(UserEntity user, string email, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> ResetAsync(UserEntity user, string currentEmail, string newEmail,
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> RemoveAsync(UserEntity user, string email,
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> AddAsync(UserEntity user, string email, 
        EmailType type, CancellationToken cancellationToken = default);
    
    public ValueTask<bool> IsTakenAsync(string email, CancellationToken cancellationToken = default);
    public ValueTask<bool> HasAsync(UserEntity user, EmailType type, CancellationToken cancellationToken = default);
    public ValueTask<bool> OwnAsync(UserEntity user, string email, CancellationToken cancellationToken = default);
}