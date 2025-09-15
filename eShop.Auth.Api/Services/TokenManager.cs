using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eShop.Domain.Common.Security.Constants;
using Microsoft.Extensions.Options;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ITokenManager), ServiceLifetime.Scoped)]
public sealed class TokenManager(
    AuthDbContext context,
    TokenHandler tokenHandler,
    IOptions<JwtOptions> options) : ITokenManager
{
    private readonly AuthDbContext context = context;
    private readonly TokenHandler tokenHandler = tokenHandler;
    private readonly JwtOptions options = options.Value;

    public async Task<string> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var verificationResult = tokenHandler.Verify();
        
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
            
            tokenHandler.Set(refreshToken, refreshTokenExpirationDate);
            
            await context.RefreshTokens.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
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

    public async Task<RefreshTokenEntity?> FindAsync(UserEntity user, CancellationToken cancellationToken = default)
    {
        var token = await context.RefreshTokens.FirstOrDefaultAsync(
            x => x.UserId == user.Id, cancellationToken);
        
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