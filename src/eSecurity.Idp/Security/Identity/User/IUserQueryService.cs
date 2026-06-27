using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Identity.User;

public interface IUserQueryService
{
    ValueTask<UserEntity?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    ValueTask<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

    ValueTask<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    ValueTask<UserEntity?> GetBySubjectAsync(string subject, CancellationToken cancellationToken = default);
    
    ValueTask<UserEntity?> GetByLoginAsync(string login, CancellationToken cancellationToken = default);

    ValueTask<bool> ExistsAsync(string username, CancellationToken cancellationToken = default);
}