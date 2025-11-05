using System.Security.Claims;

namespace eSecurity.Security.Cryptography.Tokens.Jwt;

public interface ITokenFactory
{
    public string Create(IEnumerable<Claim> claims);
}