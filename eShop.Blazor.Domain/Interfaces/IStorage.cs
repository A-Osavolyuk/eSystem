namespace eShop.Blazor.Domain.Interfaces;

public interface IStorage
{
    public ValueTask<T?> GetAsync<T>(string key);
    public ValueTask<bool> ExistsAsync(string key);
    public ValueTask SetAsync<T>(string key, T value);
    public ValueTask RemoveAsync<T>(string key);
    public ValueTask ClearAsync();
}