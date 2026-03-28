using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eSystem.Core.Enums;
using eSystem.Core.Security.Authorization.OAuth.Constants;

namespace eSecurity.Server.Security.Cryptography.Signing;

public class JwtSigner() : IJwtSigner
{
    public string Sign(IEnumerable<Claim> claims, SigningCredentials signingCredentials, JwtTokenType tokenType)
    {
        var payload = new JwtPayload(claims);
        var header = new JwtHeader(signingCredentials)
        {
            ["typ"] = EnumHelper.GetString(tokenType)
        };
        var token = new JwtSecurityToken(header, payload);
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(token);
    }
}