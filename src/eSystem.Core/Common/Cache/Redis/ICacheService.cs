using Microsoft.Extensions.Caching.Distributed;

namespace eSystem.Core.Common.Cache.Redis;

public interface ICacheService
{
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    public Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default);
    public Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options, 
        CancellationToken cancellationToken = default);
    
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    public Task RefreshAsync(string key, CancellationToken cancellationToken = default);
}