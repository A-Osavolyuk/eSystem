using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.Password;

public interface IPasswordCommandService
{
    ValueTask<Result> AddAsync(Guid userId, string password, CancellationToken cancellationToken = default);

    ValueTask<Result> ChangeAsync(Guid userId, string currentPassword, string newPassword, 
        CancellationToken cancellationToken = default);

    ValueTask<Result> ResetAsync(Guid userId, string password, CancellationToken cancellationToken = default);

    ValueTask<Result> VerifyAsync(Guid userId, string password, CancellationToken cancellationToken = default);
}