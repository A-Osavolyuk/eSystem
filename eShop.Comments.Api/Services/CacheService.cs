namespace eShop.Comments.Api.Services;

public class CacheService(IDistributedCache cache) : ICacheService
{
    private readonly IDistributedCache cache = cache;

    public async ValueTask<bool> IsKeyExistsAsync(string key)
    {
        var data = await cache.GetStringAsync(key);

        return data is null;
    }

    public async ValueTask<T?> GetAsync<T>(string key)
    {
        var data = await cache.GetStringAsync(key);

        if (data is null)
        {
            return default(T);
        }

        await cache.RefreshAsync(key);

        var result = JsonConvert.DeserializeObject<T>(data);

        return result;
    }

    public async ValueTask SetAsync<T>(string key, T value, TimeSpan expirationTime)
    {
        var json = JsonConvert.SerializeObject(value);

        await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions()
        {
            AbsoluteExpirationRelativeToNow = expirationTime
        });
    }

    public async ValueTask RemoveAsync(string key) => await cache.RemoveAsync(key);
}