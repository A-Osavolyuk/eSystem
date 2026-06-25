using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public interface ITwoFactorQueryService
{
    ValueTask<List<UserTwoFactorMethodEntity>> ListByUserAsync(Guid userId,
        CancellationToken cancellationToken = default);

    ValueTask<UserTwoFactorMethodEntity?> GetByMethodAsync(Guid userId, TwoFactorMethod method,
        CancellationToken cancellationToken = default);
}