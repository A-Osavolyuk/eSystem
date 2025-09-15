using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eShop.Domain.Common.Security;
using eShop.Domain.Common.Security.Constants;
using Microsoft.Extensions.Options;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ITokenManager), ServiceLifetime.Scoped)]
public sealed class TokenManager(
    AuthDbContext context,
    IOptions<JwtOptions> options,
    ICookieAccessor cookieAccessor) : ITokenManager
{
    private readonly AuthDbContext context = context;
    private readonly ICookieAccessor cookieAccessor = cookieAccessor;
    private readonly JwtOptions options = options.Value;
    private const string Key = "RefreshToken";

    public Result Verify()
    {
        var token = cookieAccessor.Get(Key);
        if (string.IsNullOrEmpty(token)) return Results.NotFound("Token not found");

        var handler = new JwtSecurityTokenHandler();

        var rawToken = handler.ReadJwtToken(token);
        if (rawToken is null || !rawToken.Claims.Any()) return Results.BadRequest("Invalid token");

        var expClaim = rawToken.Claims.SingleOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);
        if (expClaim is null) return Results.BadRequest("Invalid token");

        var expMilliseconds = long.Parse(expClaim.Value);
        var expDate = DateTimeOffset.FromUnixTimeMilliseconds(expMilliseconds);
        if (expDate < DateTimeOffset.UtcNow) return Results.BadRequest("Token is expired");
        
        return Result.Success();
    }

    public async Task<string> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var verificationResult = Verify();
        
        if (!verificationResult.Succeeded)
        {
            var existingEntity = await context.RefreshTokens.FirstOrDefaultAsync(
                x => x.UserId == user.Id, cancellationToken);

            if (existingEntity is not null)
            {
                context.RefreshTokens.Remove(existingEntity);
            }

            var refreshTokenClaims = new List<Claim>()
            {
                new(AppClaimTypes.Subject, user.Id.ToString()),
                new(AppClaimTypes.Jti, Guid.CreateVersion7().ToString())
            };
            
            var refreshTokenExpirationDate = DateTime.UtcNow.AddDays(options.RefreshTokenExpirationDays);
            var refreshToken = Generate(refreshTokenClaims, refreshTokenExpirationDate);
            
            var entity = new RefreshTokenEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                Token = refreshToken,
                ExpireDate = refreshTokenExpirationDate,
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            };
            
            await context.RefreshTokens.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var cookieOptions = new CookieOptions()
            {
                Path = "/",
                SameSite = SameSiteMode.Lax,
                HttpOnly = true,
                Expires = refreshTokenExpirationDate,
            };

            cookieAccessor.Set(Key, refreshToken, cookieOptions);
        }
        
        var accessTokenClaims = new List<Claim>()
        {
            new(AppClaimTypes.Subject, user.Id.ToString()),
            new(AppClaimTypes.Jti, Guid.CreateVersion7().ToString())
        };
        
        var accessTokenExpirationDate = DateTime.UtcNow.AddMinutes(options.AccessTokenExpirationMinutes);
        var token = Generate(accessTokenClaims, accessTokenExpirationDate);

        return token;
    }

    private string Generate(IEnumerable<Claim> claims, DateTime expirationDate)
    {
        const string algorithm = SecurityAlgorithms.HmacSha256Signature;

        var key = Encoding.UTF8.GetBytes(options.Secret);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm);

        var securityToken = new JwtSecurityToken(
            audience: options.Audience,
            issuer: options.Issuer,
            claims: claims,
            expires: expirationDate,
            signingCredentials: signingCredentials);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(securityToken);
        
        return token;
    }
}