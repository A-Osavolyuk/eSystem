using System.Security.Claims;
using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Server.Security.Cryptography.Signing;

public interface IJwtSigner
{
    public string Sign(IEnumerable<Claim> claims, SigningCredentials signingCredentials, JwtTokenType tokenType);
}