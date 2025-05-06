using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;

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
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), Algorithm);
        var claims = await GetClaimsAsync(userEntity);

        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes);
        var refreshTokenExpiration = DateTime.UtcNow.AddMinutes(options.ExpirationDays);

        var accessToken = WriteToken(claims, signingCredentials, accessTokenExpiration);
        var refreshToken = WriteToken(claims, signingCredentials, refreshTokenExpiration);

        await CreateAsync(userEntity, refreshToken, refreshTokenExpiration, cancellationToken);

        return new Token()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }


    public async Task<Token> RefreshAsync(UserEntity userEntity, SecurityTokenEntity tokenEntity,
        CancellationToken cancellationToken = default)
    {
        var key = Encoding.UTF8.GetBytes(options.Key);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), Algorithm);
        var claims = await GetClaimsAsync(userEntity);

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
        
        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(securityToken);
        
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
        var permissions = await permissionManager.GetAllAsync(userEntity);
        
        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x.Name!)));;
        claims.AddRange(permissions.Select(x => new Claim(ClaimTypes.Permission, x.Name!)));;

        return claims;
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