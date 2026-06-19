using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Identity.User;

public interface ICurrentUserAccessor
{
    ValueTask<TypedResult<UserEntity>> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}