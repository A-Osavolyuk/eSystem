using eSecurity.Idp.Data.Entities;
using eSecurity.Core.Security.Authentication.TwoFactor;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.TwoFactor;

public interface ITwoFactorManager
{
    public ValueTask<List<UserTwoFactorMethodEntity>> GetAllAsync(UserEntity user, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<UserTwoFactorMethodEntity?> GetAsync(UserEntity user, 
        TwoFactorMethod method, CancellationToken cancellationToken = default);
    
    public ValueTask<UserTwoFactorMethodEntity?> GetPreferredAsync(UserEntity user, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> SubscribeAsync(UserEntity user, TwoFactorMethod method, bool preferred = false,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> UnsubscribeAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<Result> UnsubscribeAsync(UserTwoFactorMethodEntity method,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> PreferAsync(UserEntity user,
        TwoFactorMethod method, CancellationToken cancellationToken = default);
    
    public ValueTask<bool> IsEnabledAsync(UserEntity user, CancellationToken cancellationToken = default);

    public ValueTask<bool> HasMethodAsync(UserEntity user, TwoFactorMethod method,
        CancellationToken cancellationToken = default);
}