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

    public async Task<string> GenerateAsync(UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        var storedRefreshToken = tokenHandler.Get();
        var verificationResult = tokenHandler.Verify(storedRefreshToken);
        
        if (!verificationResult.Succeeded)
        {
            var existingEntity = await context.RefreshTokens.FirstOrDefaultAsync(
                x => x.DeviceId == device.Id, cancellationToken);

            if (existingEntity is not null)
            {
                context.RefreshTokens.Remove(existingEntity);
            }

            var refreshTokenClaims = new List<Claim>()
            {
                new(AppClaimTypes.Subject, device.UserId.ToString()),
                new(AppClaimTypes.Jti, Guid.CreateVersion7().ToString())
            };
            
            var refreshTokenExpirationDate = DateTime.UtcNow.AddDays(options.RefreshTokenExpirationDays);
            var refreshToken = Generate(refreshTokenClaims, refreshTokenExpirationDate);
            
            var entity = new RefreshTokenEntity()
            {
                Id = Guid.CreateVersion7(),
                DeviceId = device.Id,
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
            new(AppClaimTypes.Subject, device.UserId.ToString()),
            new(AppClaimTypes.Jti, Guid.CreateVersion7().ToString())
        };
        
        var accessTokenExpirationDate = DateTime.UtcNow.AddMinutes(options.AccessTokenExpirationMinutes);
        var token = Generate(accessTokenClaims, accessTokenExpirationDate);

        return token;
    }

    public async Task<RefreshTokenEntity?> FindAsync(
        UserDeviceEntity device, CancellationToken cancellationToken = default)
    {
        var token = await context.RefreshTokens.FirstOrDefaultAsync(
            x => x.DeviceId == device.Id, cancellationToken);
        
        return token;
    }

    public async Task<Result> RemoveAsync(RefreshTokenEntity token, CancellationToken cancellationToken = default)
    {
        context.RefreshTokens.Remove(token);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
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