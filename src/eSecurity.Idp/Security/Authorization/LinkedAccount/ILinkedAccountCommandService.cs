using eSecurity.Core.Security.Authorization.OAuth;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.LinkedAccount;

public interface ILinkedAccountCommandService
{
    ValueTask<Result> CreateAsync(Guid userId, LinkedAccountType type, CancellationToken cancellationToken = default);

    ValueTask<Result> RemoveAsync(Guid linkedAccountId, CancellationToken cancellationToken = default);
}