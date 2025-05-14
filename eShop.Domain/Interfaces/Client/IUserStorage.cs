using System.Security.Claims;
using User = eShop.Domain.Models.User;

namespace eShop.Domain.Interfaces.Client;

public interface IUserStorage
{
    public ValueTask SaveAsync(List<Claim> claims);
    public ValueTask<User?> GetAsync();
    public ValueTask ClearAsync();
}