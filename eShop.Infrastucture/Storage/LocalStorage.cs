namespace eShop.Infrastructure.Storage;

public class LocalStorage(ILocalStorageService localStorage) : IStorage
{
    private readonly ILocalStorageService localStorage = localStorage;

    public async ValueTask<T?> GetAsync<T>(string key)
    {
        var item = await localStorage.GetItemAsync<T>(key);

        return item;
    }

    public async ValueTask<bool> ExistsAsync(string key)
    {
        return await localStorage.ContainKeyAsync(key);
    }

    public async ValueTask SetAsync<T>(string key, T value)
    {
        await localStorage.SetItemAsync(key, value);
    }

    public async ValueTask RemoveAsync<T>(string key)
    {
        if (await localStorage.ContainKeyAsync(key))
        {
            await localStorage.RemoveItemAsync(key);
        }
    }

    public async ValueTask ClearAsync()
    {
        await localStorage.ClearAsync();
    }
}