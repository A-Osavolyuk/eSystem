using Blazored.LocalStorage;

namespace eSecurity.Client.Common.Storage.Local;

public class LocalStorage(ILocalStorageService localStorage) : IStorage
{
    private readonly ILocalStorageService _localStorage = localStorage;

    public async ValueTask<T?> GetAsync<T>(string key)
    {
        var item = await _localStorage.GetItemAsync<T>(key);

        return item;
    }

    public async ValueTask<bool> ExistsAsync(string key)
    {
        return await _localStorage.ContainKeyAsync(key);
    }

    public async ValueTask SetAsync<T>(string key, T value)
    {
        await _localStorage.SetItemAsync(key, value);
    }

    public async ValueTask RemoveAsync<T>(string key)
    {
        if (await _localStorage.ContainKeyAsync(key))
        {
            await _localStorage.RemoveItemAsync(key);
        }
    }

    public async ValueTask ClearAsync()
    {
        await _localStorage.ClearAsync();
    }
}