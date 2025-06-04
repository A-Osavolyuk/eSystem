using System.Security.Claims;
using eShop.Domain.Stores;

namespace eShop.Domain.Interfaces.Client;

public interface IUserStorage
{
    public ValueTask SaveAsync(UserStore store);
    public ValueTask<UserStore?> GetAsync();
    public ValueTask ClearAsync();
}