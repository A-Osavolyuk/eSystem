namespace eShop.Infrastructure.Storage;

public class UserStorage(ILocalStorageService localStorage) : IUserStorage
{
    private readonly ILocalStorageService localStorage = localStorage;

    private const string UserKey = "userId";

    public async ValueTask SaveAsync(Guid userId)
    {
        if (!await localStorage.ContainKeyAsync(UserKey))
        {
            await localStorage.SetItemAsync(UserKey, userId);
        }
    }

    public async ValueTask<Guid> GetAsync()
    {
        var userId = await localStorage.GetItemAsync<Guid>(UserKey);
        return userId;   
    }

    public async ValueTask ClearAsync() => await localStorage.RemoveItemAsync(UserKey);
}