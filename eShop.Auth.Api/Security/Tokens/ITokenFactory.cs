using System.Security.Claims;

namespace eShop.Auth.Api.Security.Tokens;

public interface ITokenFactory
{
    public string Create(IEnumerable<Claim> claims);
}