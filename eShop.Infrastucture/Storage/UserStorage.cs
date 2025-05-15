using eShop.Domain.Models;
using ClaimTypes = eShop.Domain.Common.Security.ClaimTypes;

namespace eShop.Infrastructure.Storage;

public class UserStorage(ILocalStorageService localStorage) : IUserStorage
{
    private readonly ILocalStorageService localStorage = localStorage;

    private const string UserKey = "user";

    public async ValueTask SaveAsync(List<Claim> claims)
    {
        if (!await localStorage.ContainKeyAsync(UserKey))
        {
            var user = new UserModel
            {
                Id = Guid.Parse(claims.First(x => x.Type == ClaimTypes.Id).Value),
                Email = claims.First(x => x.Type == ClaimTypes.Email).Value,
                UserName = claims.First(x => x.Type == ClaimTypes.UserName).Value,
                PhoneNumber = claims.FirstOrDefault(x => x.Type == ClaimTypes.PhoneNumber)?.Value ?? string.Empty
            };
        
            await localStorage.SetItemAsync(UserKey, user);
        }
    }

    public async ValueTask<UserModel?> GetAsync()
    {
        var user = await localStorage.GetItemAsync<UserModel>(UserKey);
        return user;   
    }

    public async ValueTask ClearAsync() => await localStorage.RemoveItemAsync(UserKey);
}