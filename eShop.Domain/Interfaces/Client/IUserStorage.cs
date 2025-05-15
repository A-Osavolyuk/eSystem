using System.Security.Claims;
using eShop.Domain.Models;

namespace eShop.Domain.Interfaces.Client;

public interface IUserStorage
{
    public ValueTask SaveAsync(List<Claim> claims);
    public ValueTask<UserModel?> GetAsync();
    public ValueTask ClearAsync();
}