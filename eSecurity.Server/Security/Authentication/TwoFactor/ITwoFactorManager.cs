using eSecurity.Core.Security.Authentication.TwoFactor;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.TwoFactor;

public interface ITwoFactorManager
{
    public ValueTask<UserTwoFactorMethodEntity?> GetAsync(UserEntity user, 
        TwoFactorMethod method, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> SubscribeAsync(UserEntity user, TwoFactorMethod method, bool preferred = false,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> UnsubscribeAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> UnsubscribeAsync(UserTwoFactorMethodEntity method,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> PreferAsync(UserEntity user,
        TwoFactorMethod method, CancellationToken cancellationToken = default);
}