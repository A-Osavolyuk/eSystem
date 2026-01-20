using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace eSecurity.Server.Security.Cryptography.Signing;

public class JwtSigner() : IJwtSigner
{
    public string Sign(IEnumerable<Claim> claims, SigningCredentials signingCredentials, string tokenType)
    {
        var payload = new JwtPayload(claims);
        var header = new JwtHeader(signingCredentials)
        {
            ["typ"] = tokenType
        };
        var token = new JwtSecurityToken(header, payload);
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(token);
    }
}