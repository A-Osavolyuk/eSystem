using System.Security.Claims;

namespace eShop.Domain.Interfaces.Client;

public interface IUserStorage
{
    public ValueTask SaveAsync(List<Claim> claims);
    public ValueTask<UserStore?> GetAsync();
    public ValueTask ClearAsync();
}