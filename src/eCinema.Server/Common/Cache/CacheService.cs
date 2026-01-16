using System.Text.Json;
using eSystem.Core.Common.Cache.Redis;
using Microsoft.Extensions.Caching.Distributed;

namespace eCinema.Server.Common.Cache;

public class CacheService(IDistributedCache distributedCache) : ICacheService
{
    private readonly IDistributedCache _distributedCache = distributedCache;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var json = await _distributedCache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrEmpty(json))
            return default;

        var value = JsonSerializer.Deserialize<T>(json);
        return value;
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, json, cancellationToken);
    }

    public async Task SetAsync<T>(string key, T value, DistributedCacheEntryOptions options,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(value);
        await _distributedCache.SetStringAsync(key, json, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        => await _distributedCache.RemoveAsync(key, cancellationToken);

    public async Task RefreshAsync(string key, CancellationToken cancellationToken = default)
        => await _distributedCache.RefreshAsync(key, cancellationToken);
}