using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eShop.Auth.Api.Security.Tokens;
using Microsoft.Extensions.Options;

namespace eShop.Auth.Api.Security.Jwt;

public class JwtTokenFactory(IOptions<JwtOptions> options) : ITokenFactory
{
    private readonly JwtOptions options = options.Value;
    
    public string Create(IEnumerable<Claim> claims)
    {
        const string algorithm = SecurityAlgorithms.HmacSha256Signature;

        var expirationDate = DateTime.UtcNow.AddMinutes(options.AccessTokenExpirationMinutes);
        var key = Encoding.UTF8.GetBytes(options.Secret);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm);

        var securityToken = new JwtSecurityToken(
            audience: options.Audience,
            issuer: options.Issuer,
            claims: claims,
            expires: expirationDate,
            signingCredentials: signingCredentials);

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(securityToken);
    }
}