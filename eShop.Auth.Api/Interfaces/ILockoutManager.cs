namespace eShop.Auth.Api.Interfaces;

public interface ILockoutManager
{
    public ValueTask<Result> BlockAsync(
        UserEntity userEntity,
        LockoutType type,
        string? description = null, 
        bool permanent = false,
        TimeSpan? duration = null, 
        DateTimeOffset? endDate = null, 
        CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UnblockAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
}