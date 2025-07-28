using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eShop.Domain.Common.Security;
using Microsoft.Extensions.Options;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ITokenManager), ServiceLifetime.Scoped)]
public sealed class TokenManager(
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

    public async ValueTask<Result> VerifyAsync(UserEntity user, string token,
        CancellationToken cancellationToken = default)
    {
        var entity = await context.RefreshTokens
            .FirstOrDefaultAsync(x => x.UserId == user.Id && x.Token == token,
                cancellationToken: cancellationToken);

        if (entity is null)
        {
            return Results.NotFound("Token not found");
        }

        if (entity.ExpireDate < DateTime.UtcNow)
        {
            return Results.BadRequest("Token already expired");
        }

        context.RefreshTokens.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async Task<string> GenerateAsync(UserEntity user, TokenType type,
        CancellationToken cancellationToken = default)
    {
        var key = Encoding.UTF8.GetBytes(options.Secret);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), Algorithm);

        var expirationDate = type switch
        {
            TokenType.Access => DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes),
            TokenType.Refresh => DateTime.UtcNow.AddDays(options.ExpirationDays),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        var claims = GetClaims(user);
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

        if (type is TokenType.Access)
        {
            return token;
        }
        
        var existingEntity = await context.RefreshTokens.FirstOrDefaultAsync(
            x => x.UserId == user.Id, cancellationToken: cancellationToken);

        if (existingEntity is null)
        {
            var entity = new RefreshTokenEntity()
            {
                Id = jti,
                UserId = user.Id,
                Token = token,
                ExpireDate = expirationDate,
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            };

            await context.RefreshTokens.AddAsync(entity, cancellationToken);
        }
        else
        {
            var entity = new RefreshTokenEntity()
            {
                Id = jti,
                UserId = user.Id,
                Token = token,
                ExpireDate = expirationDate,
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            };

            context.RefreshTokens.Remove(existingEntity);
            await context.RefreshTokens.AddAsync(entity, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);

        return token;
    }

    private List<Claim> GetClaims(UserEntity user)
    {
        var claims = new List<Claim>()
        {
            new(AppClaimTypes.Id, user.Id.ToString()),
        };

        var roles = user.Roles.Select(x => x.Role);
        var permissions = user.Permissions.Select(x => x.Permission);

        claims.AddRange(roles.Select(x => new Claim(AppClaimTypes.Role, x.Name!)));
        claims.AddRange(permissions.Select(x => new Claim(AppClaimTypes.Permission, x.Name!)));

        return claims;
    }
}