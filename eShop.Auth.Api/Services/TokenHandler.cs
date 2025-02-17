using eShop.Auth.Api.Entities;
using ClaimTypes = eShop.Domain.Common.Security.ClaimTypes;

namespace eShop.Auth.Api.Services;

internal sealed class TokenHandler(IOptions<JwtOptions> options, ISecurityManager securityManager) : ITokenHandler
{
    private readonly ISecurityManager securityManager = securityManager;
    private const int AccessTokenExpirationMinutes = 30;
    private readonly JwtOptions options = options.Value;
    private readonly JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

    public async Task<Token> GenerateTokenAsync(AppUser user, List<string> roles, List<string> permissions)
    {
        var key = Encoding.UTF8.GetBytes(options.Key);
        var algorithm = SecurityAlgorithms.HmacSha256Signature;
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm);

        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes);
        var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(options.ExpirationDays);

        var claims = SetClaims(user, roles, permissions);
        var accessToken = WriteToken(claims, signingCredentials, accessTokenExpiration);
        var refreshToken = WriteToken(claims, signingCredentials, refreshTokenExpiration);

        await securityManager.SaveTokenAsync(user, refreshToken, refreshTokenExpiration);

        return new Token()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }


    public string RefreshToken(string token)
    {
        var rawToken = DecryptToken(token);
        var claims = GetClaimsFromToken(rawToken);
        var key = Encoding.UTF8.GetBytes(options.Key);
        var algorithm = SecurityAlgorithms.HmacSha256Signature;
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm);
        var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(options.ExpirationDays);
        var refreshToken = WriteToken(claims, signingCredentials, refreshTokenExpiration);

        return refreshToken;
    }

    private string WriteToken(List<Claim> claims, SigningCredentials signingCredentials, DateTime expiration)
    {
        var securityToken = new JwtSecurityToken(
            audience: options.Audience,
            issuer: options.Issuer,
            claims: claims,
            expires: expiration,
            signingCredentials: signingCredentials);
        var token = handler.WriteToken(securityToken);
        return token;
    }

    private List<Claim> SetClaims(AppUser user, List<string> roles, List<string> permissions)
    {
        var claims = new List<Claim>()
        {
            new(ClaimTypes.UserName, user.UserName ?? ""),
            new(ClaimTypes.Email, user.Email ?? ""),
            new(ClaimTypes.Id, user.Id),
            new(ClaimTypes.PhoneNumber, user.PhoneNumber ?? "")
        };

        if (roles.Any())
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        if (permissions.Any())
        {
            foreach (var permission in permissions)
            {
                claims.Add(new Claim(ClaimTypes.Permission, permission));
            }
        }

        return claims;
    }

    private JwtSecurityToken? DecryptToken(string token)
    {
        if (!string.IsNullOrEmpty(token) && handler.CanReadToken(token))
        {
            var rawToken = handler.ReadJwtToken(token);

            return rawToken;
        }
        else
        {
            return null;
        }
    }

    private List<Claim> GetClaimsFromToken(JwtSecurityToken? token)
    {
        if (token is null)
        {
            return new List<Claim>();
        }

        var claims = new List<Claim>()
        {
            new(ClaimTypes.UserName, GetClaimValue(token, ClaimTypes.UserName)),
            new(ClaimTypes.Email, GetClaimValue(token, ClaimTypes.Email)),
            new(ClaimTypes.Id, GetClaimValue(token, ClaimTypes.Id)),
            new(ClaimTypes.PhoneNumber, GetClaimValue(token, ClaimTypes.PhoneNumber)),
        };

        var roles = token.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
        var permissions = token.Claims.Where(x => x.Type == ClaimTypes.Permission).Select(x => x.Value).ToList();

        if (roles.Any())
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        if (permissions.Any())
        {
            foreach (var permission in permissions)
            {
                claims.Add(new Claim(ClaimTypes.Permission, permission));
            }
        }

        return claims;
    }

    private string GetClaimValue(JwtSecurityToken token, string claimType)
    {
        var value = token.Claims.FirstOrDefault(x => x.Type == claimType)!.Value;
        return value;
    }
}