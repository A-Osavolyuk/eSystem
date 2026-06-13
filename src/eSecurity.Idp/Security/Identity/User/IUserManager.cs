using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.User;

public interface IUserManager
{
    public ValueTask<TypedResult<UserEntity>> GetUserAsync(CancellationToken cancellationToken = default);
    
    public ValueTask<UserEntity?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByUsernameAsync(string name, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindByLoginAsync(string login, CancellationToken cancellationToken = default);
    public ValueTask<UserEntity?> FindBySubjectAsync(string subject, CancellationToken cancellationToken = default);

    public ValueTask<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UpdateAsync(UserEntity user, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> AddResendAttemptAsync(UserEntity user, TimeSpan dueTime, 
        CancellationToken cancellationToken = default);

    public ValueTask<Result> ResetResendAttemptsAsync(UserEntity user, TimeSpan dueTime, 
        CancellationToken cancellationToken = default);

    public ValueTask<Result> CleanResendAttemptsAsync(UserEntity user, CancellationToken cancellationToken = default);
}