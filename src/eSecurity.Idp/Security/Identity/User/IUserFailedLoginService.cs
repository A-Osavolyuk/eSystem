using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.User;

public interface IUserFailedLoginService
{
    ValueTask<Result> ResetAttemptsAsync(UserEntity userEntity, CancellationToken cancellationToken = default);

    ValueTask<Result> IncrementAttemptAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
}