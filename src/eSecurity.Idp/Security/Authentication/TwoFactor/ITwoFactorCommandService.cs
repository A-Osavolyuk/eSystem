using eSecurity.Core.Security.Authentication.TwoFactor;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public interface ITwoFactorCommandService
{
    ValueTask<Result> AddMethodAsync(Guid userId, TwoFactorMethod method, 
        CancellationToken cancellationToken = default);

    ValueTask<Result> RemoveMethodAsync(Guid userId, Guid methodId, 
        CancellationToken cancellationToken = default);

    ValueTask<Result> SetPreferredMethodAsync(Guid userId, Guid methodId,
        CancellationToken cancellationToken = default);

    ValueTask<Result> ResetMethodsAsync(Guid userId, CancellationToken cancellationToken = default);
}