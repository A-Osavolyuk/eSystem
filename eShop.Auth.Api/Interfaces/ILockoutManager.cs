namespace eShop.Auth.Api.Interfaces;

public interface ILockoutManager
{
    public ValueTask<LockoutStateEntity> FindAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default);

    public ValueTask<Result> LockoutAsync(
        UserEntity userEntity,
        LockoutReasonEntity lockoutReason, 
        string description, 
        bool permanent = false,
        TimeSpan? duration = null, 
        DateTimeOffset? endDate = null, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UnlockAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
}