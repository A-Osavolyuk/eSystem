using System.Security.Claims;

namespace eSystem.Auth.Api.Security.Tokens;

public interface ITokenFactory
{
    public string Create(IEnumerable<Claim> claims);
}