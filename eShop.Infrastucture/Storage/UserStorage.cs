using eShop.Domain.Common.Security;
using eShop.Domain.Types;

namespace eShop.Infrastructure.Storage;

public class UserStorage(ILocalStorageService localStorage) : IUserStorage
{
    private readonly ILocalStorageService localStorage = localStorage;

    private const string UserKey = "user";

    public async ValueTask SaveAsync(List<Claim> claims)
    {
        if (!await localStorage.ContainKeyAsync(UserKey))
        {
            var user = new UserStore
            {
                Id = Guid.Parse(claims.First(x => x.Type == AppClaimTypes.Id).Value),
                Email = claims.First(x => x.Type == AppClaimTypes.Email).Value,
                UserName = claims.First(x => x.Type == AppClaimTypes.UserName).Value,
                PhoneNumber = claims.FirstOrDefault(x => x.Type == AppClaimTypes.PhoneNumber)?.Value ?? null
            };
        
            await localStorage.SetItemAsync(UserKey, user);
        }
    }

    public async ValueTask<UserStore?> GetAsync()
    {
        var user = await localStorage.GetItemAsync<UserStore>(UserKey);
        return user;   
    }

    public async ValueTask ClearAsync() => await localStorage.RemoveItemAsync(UserKey);
}