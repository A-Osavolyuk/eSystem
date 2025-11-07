using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace eSecurity.Security.Cryptography.Tokens.Jwt;

public class JwtSigner() : IJwtSigner
{
    public string Sign(IEnumerable<Claim> claims, SigningCredentials signingCredentials)
    {
        var securityToken = new JwtSecurityToken(claims: claims, signingCredentials: signingCredentials);
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(securityToken);
    }
}