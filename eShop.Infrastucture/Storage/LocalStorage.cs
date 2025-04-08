namespace eShop.Infrastructure.Storage;

public class LocalStorage(ILocalStorageService localStorage) : IStorage
{
    private readonly ILocalStorageService localStorage = localStorage;

    public async ValueTask<T?> GetAsync<T>(string key)
    {
        var item = await localStorage.GetItemAsync<T>(key);

        return item;
    }

    public async ValueTask UpdateAsync<T>(string key, T value)
    {
        if (await localStorage.ContainKeyAsync(key))
        {
            await localStorage.SetItemAsync(key, value);
        }
    }

    public async ValueTask CreateAsync<T>(string key, T value)
    {
        if (!await localStorage.ContainKeyAsync(key))
        {
            await localStorage.SetItemAsync(key, value);
        }
    }

    public async ValueTask DeleteAsync<T>(string key)
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