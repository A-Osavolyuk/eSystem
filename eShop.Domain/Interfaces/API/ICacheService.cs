namespace eShop.Domain.Interfaces.API;

public interface ICacheService
{
    public ValueTask<bool> IsKeyExistsAsync(string key);
    public ValueTask<T?> GetAsync<T>(string key);
    public ValueTask SetAsync<T>(string key, T value, TimeSpan expirationTime);
    public ValueTask RemoveAsync(string key);
}