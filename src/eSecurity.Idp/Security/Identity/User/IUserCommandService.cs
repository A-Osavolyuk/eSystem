using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.User;

public interface IUserCommandService
{
    ValueTask<Result> CreateAsync(UserEntity user, CancellationToken cancellationToken = default);

    ValueTask<Result> ChangeUsernameAsync(Guid userId, string username, CancellationToken cancellationToken = default);
}