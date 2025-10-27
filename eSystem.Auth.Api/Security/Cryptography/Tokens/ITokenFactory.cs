using System.Security.Claims;

namespace eSystem.Auth.Api.Security.Cryptography.Tokens;

public interface ITokenFactory
{
    public string Create(IEnumerable<Claim> claims);
}