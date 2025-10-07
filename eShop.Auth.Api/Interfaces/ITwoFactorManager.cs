namespace eShop.Auth.Api.Interfaces;

public interface ITwoFactorManager
{
    public ValueTask<Result> SubscribeAsync(UserEntity user, TwoFactorMethod method, bool isPrimary = false,
        CancellationToken cancellationToken = default);
    public ValueTask<Result> UnsubscribeAsync(UserEntity user, 
        CancellationToken cancellationToken = default);
}