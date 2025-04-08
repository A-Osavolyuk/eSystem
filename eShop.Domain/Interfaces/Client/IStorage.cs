namespace eShop.Domain.Interfaces.Client;

public interface IStorage
{
    public ValueTask<T?> GetAsync<T>(string key);
    public ValueTask UpdateAsync<T>(string key, T value);
    public ValueTask CreateAsync<T>(string key, T value);
    public ValueTask DeleteAsync<T>(string key);
    public ValueTask ClearAsync();
}