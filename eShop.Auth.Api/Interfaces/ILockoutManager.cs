namespace eShop.Auth.Api.Interfaces;

public interface ILockoutManager
{
    public ValueTask<LockoutStateEntity> FindAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> LockoutAsync(UserEntity userEntity, LockoutReason reason, string description, LockoutPeriod period, 
        DateTimeOffset? endDate = null, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UnlockAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
}