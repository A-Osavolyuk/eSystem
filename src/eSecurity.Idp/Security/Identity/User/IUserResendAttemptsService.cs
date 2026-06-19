using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.User;

public interface IUserResendAttemptsService
{
    ValueTask<Result> IncrementAttemptAsync(UserEntity user, TimeSpan dueTime,
        CancellationToken cancellationToken = default);

    ValueTask<Result> ResetAttemptsAsync(UserEntity user, TimeSpan dueTime, 
        CancellationToken cancellationToken = default);

    ValueTask<Result> CleanAttemptsAsync(UserEntity user, 
        CancellationToken cancellationToken = default);
}