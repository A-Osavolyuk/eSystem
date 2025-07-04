using eShop.Domain.Interfaces.API;
using StackExchange.Redis;

namespace eShop.Product.Api.Services;

[Injectable(typeof(ICacheService), ServiceLifetime.Scoped)]
public class CacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDatabase database = redis.GetDatabase();

    public async ValueTask<bool> IsKeyExistsAsync(string key)
    {
        return await database.KeyExistsAsync(key);
    }

    public async ValueTask<T?> GetAsync<T>(string key)
    {
        var data = await database.StringGetAsync(key);

        var result = JsonConvert.DeserializeObject<T>(data!);

        return result;
    }

    public async ValueTask SetAsync<T>(string key, T value, TimeSpan expirationTime)
    {
        var json = JsonConvert.SerializeObject(value);

        await database.StringSetAsync(key, json, expirationTime);
    }

    public async ValueTask RemoveAsync(string key) => await database.KeyDeleteAsync(key);
}