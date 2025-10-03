namespace eShop.Auth.Api.Interfaces;

public interface ITwoFactorManager
{
    public ValueTask<TwoFactorMethodEntity?> FindByTypeAsync(MethodType type, 
        CancellationToken cancellationToken = default);
    public ValueTask<Result> SubscribeAsync(UserEntity user, MethodType type, bool isPrimary = false,
        CancellationToken cancellationToken = default);
    public ValueTask<Result> UnsubscribeAsync(UserEntity user, UserTwoFactorMethodEntity method, 
        CancellationToken cancellationToken = default);
}