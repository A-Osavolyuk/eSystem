namespace eShop.Auth.Api.Interfaces;

public interface ILockoutManager
{
    public ValueTask<Result> BlockPermanentlyAsync(UserEntity user, LockoutType type,
        string? description = null, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> BlockTemporaryAsync(UserEntity user, LockoutType type, LockoutPeriod period,
        string? description = null, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> BlockTemporaryAsync(UserEntity user, LockoutType type, TimeSpan duration,
        string? description = null, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UnblockAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
}