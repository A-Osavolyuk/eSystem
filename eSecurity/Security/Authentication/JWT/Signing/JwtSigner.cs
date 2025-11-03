using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eSystem.Core.Security.Authentication.JWT;

namespace eSecurity.Security.Authentication.JWT.Signing;

public class JwtSigner : IJwtSigner
{
    public string Sign(IEnumerable<Claim> claims, string secret, string algorithm)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm);
        var securityToken = new JwtSecurityToken(claims: claims, signingCredentials: signingCredentials);
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(securityToken);
    }
}