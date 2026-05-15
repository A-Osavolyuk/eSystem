using System.Text.Json;
using StackExchange.Redis;

namespace eSecurity.Client.BFF.Common.Cache;

public sealed class RedisCache(IConnectionMultiplexer connectionMultiplexer) : ICache
{
    private readonly IDatabase _database = connectionMultiplexer.GetDatabase();
    
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));
        
        var json = await _database.StringGetAsync(key);
        if (string.IsNullOrEmpty(json))
            return default;

        var jsonString = json.ToString();
        if (string.IsNullOrEmpty(jsonString))
            return default;

        try
        {
            return JsonSerializer.Deserialize<T>(jsonString);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(key))
            throw new ArgumentNullException(nameof(key));
        
        if (value is null)
            throw new ArgumentNullException(nameof(value));

        var json = JsonSerializer.Serialize(value);
        if (ttl.HasValue)
            await _database.StringSetAsync(key, json, ttl.Value);
        else
            await _database.StringSetAsync(key, json);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task RenewAsync(string key, TimeSpan ttl, CancellationToken cancellationToken = default)
    {
        await _database.KeyExpireAsync(key, ttl);
    }
}