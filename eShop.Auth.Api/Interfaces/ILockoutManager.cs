namespace eShop.Auth.Api.Interfaces;

public interface ILockoutManager
{
    public ValueTask<LockoutStatus> GetStatusAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> EnableAsync(UserEntity user, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    public ValueTask<Result> DisableAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> SetEndDateAsync(UserEntity user, DateTimeOffset endDate, CancellationToken cancellationToken = default);
}