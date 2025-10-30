using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eSystem.Core.Security.Authentication.JWT;
using eSystem.Core.Security.Cryptography.Tokens;

namespace eSystem.Auth.Api.Security.Authentication.JWT;

public class JwtTokenFactory(IOptions<JwtOptions> options) : ITokenFactory
{
    private readonly JwtOptions options = options.Value;
    
    public string Create(IEnumerable<Claim> claims)
    {
        const string algorithm = SecurityAlgorithms.HmacSha256;

        var expirationDate = DateTime.UtcNow.AddMinutes(options.AccessTokenExpirationMinutes);
        var key = Encoding.UTF8.GetBytes(options.Secret);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm);

        var securityToken = new JwtSecurityToken(
            issuer: options.Issuer,
            claims: claims,
            expires: expirationDate,
            signingCredentials: signingCredentials);

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(securityToken);
    }
}