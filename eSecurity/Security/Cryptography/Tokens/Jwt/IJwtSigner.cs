using System.Security.Claims;

namespace eSecurity.Security.Cryptography.Tokens.Jwt;

public interface IJwtSigner
{
    public string Sign(IEnumerable<Claim> claims, SigningCredentials signingCredentials);
}