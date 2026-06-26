using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.Password;

public interface IPasswordQueryService
{
    ValueTask<PasswordEntity?> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    ValueTask<bool> ExistsAsync(Guid userId, CancellationToken cancellationToken = default);
}