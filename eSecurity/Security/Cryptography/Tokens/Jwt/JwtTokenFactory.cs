using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eSystem.Core.Security.Authentication.JWT;

namespace eSecurity.Security.Cryptography.Tokens.Jwt;

public class JwtTokenFactory(IOptions<JwtOptions> options) : ITokenFactory
{
    private readonly JwtOptions options = options.Value;

    public string Create(IEnumerable<Claim> claims)
    {
        const string algorithm = SecurityAlgorithms.HmacSha256;
        var key = Encoding.UTF8.GetBytes(options.Secret);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm);
        var securityToken = new JwtSecurityToken(claims: claims, signingCredentials: signingCredentials);
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(securityToken);
    }
}