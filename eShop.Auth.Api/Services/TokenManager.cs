using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eShop.Auth.Api.Enums;
using Microsoft.Extensions.Options;
using SecurityToken = eShop.Domain.Types.SecurityToken;

namespace eShop.Auth.Api.Services;

public class TokenManager(
    AuthDbContext context,
    IRoleManager roleManager,
    IPermissionManager permissionManager,
    IOptions<JwtOptions> options) : ITokenManager
{
    private readonly AuthDbContext context = context;
    private readonly IRoleManager roleManager = roleManager;
    private readonly IPermissionManager permissionManager = permissionManager;
    private readonly JwtOptions options = options.Value;
    private const int AccessTokenExpirationMinutes = 30;
    private const string Algorithm = SecurityAlgorithms.HmacSha256Signature;

    public async ValueTask<RefreshTokenEntity?> FindAsync(UserEntity userEntity,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.RefreshTokens
            .FirstOrDefaultAsync(x => x.UserId == userEntity.Id, cancellationToken: cancellationToken);

        return entity;
    }

    public async ValueTask<Result> RemoveAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var token = await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == userEntity.Id, cancellationToken: cancellationToken);

        if (token is null)
        {
            return Results.NotFound("Token not found");
        }

        context.RefreshTokens.Remove(token);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<SecurityToken> GenerateAsync(UserEntity userEntity, CancellationToken cancellationToken = default)
    {
        var key = Encoding.UTF8.GetBytes(options.Key);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), Algorithm);
        
        var accessToken = await GenerateTokenAsync(signingCredentials, userEntity, TokenType.Access);
        var refreshToken = await GenerateTokenAsync(signingCredentials, userEntity, TokenType.Refresh);

        return new SecurityToken()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private async Task<string> GenerateTokenAsync(SigningCredentials signingCredentials, UserEntity user, TokenType type)
    {
        var expirationDate = type switch
        {
            TokenType.Access => DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes),
            TokenType.Refresh => DateTime.UtcNow.AddDays(options.ExpirationDays),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        var claims = await GetClaimsAsync(user);
        var jti = Guid.NewGuid();
        var jtiClaim = new Claim(JwtRegisteredClaimNames.Jti, jti.ToString());
        
        var securityToken = new JwtSecurityToken(
            audience: options.Audience,
            issuer: options.Issuer,
            claims: [..claims, jtiClaim],
            expires: expirationDate,
            signingCredentials: signingCredentials);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(securityToken);

        if (type is TokenType.Refresh)
        {
            var entity = await context.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == user.Id);

            if (entity is null)
            {
                entity = new RefreshTokenEntity()
                {
                    Id = jti,
                    UserId = user.Id,
                    Token = token,
                    ExpireDate = expirationDate,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = null
                };

                await context.RefreshTokens.AddAsync(entity);
            }
            else
            {
                var newEntity = new RefreshTokenEntity()
                {
                    Id = jti,
                    UserId = user.Id,
                    Token = token,
                    ExpireDate = expirationDate,
                    CreateDate = DateTime.UtcNow,
                    UpdateDate = null
                };
                
                context.RefreshTokens.Remove(entity);
                await context.RefreshTokens.AddAsync(newEntity);
            }

            await context.SaveChangesAsync();
        }
        
        return token;
    }

    private async Task<List<Claim>> GetClaimsAsync(UserEntity userEntity)
    {
        var claims = new List<Claim>()
        {
            new(ClaimTypes.UserName, userEntity.UserName!),
            new(ClaimTypes.Email, userEntity.Email!),
            new(ClaimTypes.Id, userEntity.Id.ToString()),
            new(ClaimTypes.PhoneNumber, userEntity.PhoneNumber ?? "")
        };
        
        var roles = await roleManager.GetByUserAsync(userEntity);
        var permissions = await permissionManager.GetByUserAsync(userEntity);
        
        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x.Name!)));;
        claims.AddRange(permissions.Select(x => new Claim(ClaimTypes.Permission, x.Name!)));;

        return claims;
    }
}