namespace eSecurity.Server.Common.Cache;

public interface ICache
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RenewAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default);
}