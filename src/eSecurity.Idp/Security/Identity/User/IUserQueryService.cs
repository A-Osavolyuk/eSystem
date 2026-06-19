using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Identity.User;

public interface IUserQueryService
{
    ValueTask<UserEntity?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);

    ValueTask<UserEntity?> GetByUsernameAsync(string username, CancellationToken cancellationToken);

    ValueTask<UserEntity?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    ValueTask<UserEntity?> GetBySubjectAsync(string subject, CancellationToken cancellationToken);
    
    ValueTask<UserEntity?> GetByLoginAsync(string login, CancellationToken cancellationToken);
}