using eShop.Domain.Common.Security;
using eShop.Domain.Stores;
using eShop.Domain.Types;

namespace eShop.Infrastructure.Storage;

public class UserStorage(ILocalStorageService localStorage) : IUserStorage
{
    private readonly ILocalStorageService localStorage = localStorage;

    private const string UserKey = "user";

    public async ValueTask SaveAsync(UserStore store)
    {
        if (!await localStorage.ContainKeyAsync(UserKey))
        {
            await localStorage.SetItemAsync(UserKey, store);
        }
    }

    public async ValueTask<UserStore?> GetAsync()
    {
        var user = await localStorage.GetItemAsync<UserStore>(UserKey);
        return user;   
    }

    public async ValueTask ClearAsync() => await localStorage.RemoveItemAsync(UserKey);
}