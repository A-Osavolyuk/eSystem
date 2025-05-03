using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace eShop.Auth.Api.Services;

public class TokenManager(
    AuthDbContext context,
    IOptions<JwtOptions> options) : ITokenManager
{
    private readonly AuthDbContext context = context;
    private const int AccessTokenExpirationMinutes = 30;
    private readonly JwtOptions options = options.Value;
    private readonly JwtSecurityTokenHandler handler = new();

    public async ValueTask<SecurityTokenEntity?> FindAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.SecurityTokens
            .FirstOrDefaultAsync(x => x.UserId == userEntity.Id, cancellationToken: cancellationToken);

        return entity;
    }

    public async ValueTask<Result> RemoveAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var token = await context.SecurityTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userEntity.Id, cancellationToken: cancellationToken);

        if (token is null)
        {
            return Results.NotFound("Token not found");
        }

        context.SecurityTokens.Remove(token);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<Token> GenerateAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var key = Encoding.UTF8.GetBytes(options.Key);
        var algorithm = SecurityAlgorithms.HmacSha256Signature;
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm);

        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes);
        var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(options.ExpirationDays);

        var claims = SetClaims(userEntity);
        var accessToken = WriteToken(claims, signingCredentials, accessTokenExpiration);
        var refreshToken = WriteToken(claims, signingCredentials, refreshTokenExpiration);

        await CreateAsync(userEntity, refreshToken, refreshTokenExpiration);

        return new Token()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }


    public async Task<Token> RefreshAsync(UserEntity userEntity, SecurityTokenEntity tokenEntity, CancellationToken cancellationToken = default)
    {
        var rawToken = DecryptToken(tokenEntity.Token);
        var claims = GetClaimsFromToken(rawToken);
        var key = Encoding.UTF8.GetBytes(options.Key);
        var algorithm = SecurityAlgorithms.HmacSha256Signature;
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm);
        
        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes);
        var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(options.ExpirationDays);
        
        var accessToken = WriteToken(claims, signingCredentials, accessTokenExpiration);
        var refreshToken = WriteToken(claims, signingCredentials, refreshTokenExpiration);

        tokenEntity.ExpireDate = refreshTokenExpiration;
        tokenEntity.Token = refreshToken;
        
        await UpdateAsync(tokenEntity, cancellationToken);

        return new Token()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
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

    private List<Claim> SetClaims(UserEntity userEntity)
    {
        var claims = new List<Claim>()
        {
            new(ClaimTypes.UserName, userEntity.UserName ?? ""),
            new(ClaimTypes.Email, userEntity.Email ?? ""),
            new(ClaimTypes.Id, userEntity.Id.ToString()),
            new(ClaimTypes.PhoneNumber, userEntity.PhoneNumber ?? "")
        };

        return claims;
    }

    private JwtSecurityToken? DecryptToken(string token)
    {
        if (!string.IsNullOrEmpty(token) && handler.CanReadToken(token))
        {
            var rawToken = handler.ReadJwtToken(token);

            return rawToken;
        }

        return null;
    }

    private List<Claim> GetClaimsFromToken(JwtSecurityToken? token)
    {
        if (token is null)
        {
            return [];
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

    private async ValueTask<Result> CreateAsync(UserEntity userEntity, string token, DateTime tokenExpiration,
        CancellationToken cancellationToken = default)
    {
        var entity = new SecurityTokenEntity()
        {
            UserId = userEntity.Id,
            Token = token,
            ExpireDate = tokenExpiration,
        };

        await context.SecurityTokens.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    
    private async ValueTask<Result> UpdateAsync(SecurityTokenEntity tokenEntity,
        CancellationToken cancellationToken = default)
    {
        context.SecurityTokens.Update(tokenEntity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}