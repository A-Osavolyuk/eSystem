using System.Security.Claims;

namespace eSecurity.Server.Security.Cryptography.Signing;

public interface IJwtSigner
{
    public string Sign(IEnumerable<Claim> claims, SigningCredentials signingCredentials);
}