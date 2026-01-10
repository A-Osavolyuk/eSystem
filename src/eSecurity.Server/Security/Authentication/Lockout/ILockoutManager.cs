using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Lockout;

public interface ILockoutManager
{
    public ValueTask<UserLockoutStateEntity?> GetAsync(UserEntity user, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> BlockPermanentlyAsync(UserEntity user, LockoutType type,
        string? description = null, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> BlockTemporaryAsync(UserEntity user, LockoutType type, LockoutPeriod period,
        string? description = null, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> BlockTemporaryAsync(UserEntity user, LockoutType type, TimeSpan duration,
        string? description = null, CancellationToken cancellationToken = default);
    
    public ValueTask<Result> UnblockAsync(UserEntity user, CancellationToken cancellationToken = default);
}