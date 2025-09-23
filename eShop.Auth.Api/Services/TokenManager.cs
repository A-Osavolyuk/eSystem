using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using eShop.Auth.Api.Types;
using eShop.Domain.Common.Security.Constants;
using Microsoft.Extensions.Options;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ITokenManager), ServiceLifetime.Scoped)]
public sealed class TokenManager(
    AuthDbContext context,
    IOptions<JwtOptions> options) : ITokenManager
{
    private readonly AuthDbContext context = context;
    private readonly JwtOptions options = options.Value;

    public async Task<string> GenerateAsync(UserDeviceEntity device,
        CancellationToken cancellationToken = default)
    {
        var accessTokenExpirationData = DateTime.UtcNow.AddMinutes(options.AccessTokenExpirationMinutes);
        
        var claims = new List<Claim>()
        {
            new(AppClaimTypes.Subject, device.UserId.ToString()),
            new(AppClaimTypes.Jti, Guid.CreateVersion7().ToString())
        };

        var accessToken = Generate(claims, accessTokenExpirationData);

        var existingEntity = await context.RefreshTokens.FirstOrDefaultAsync(
            x => x.DeviceId == device.Id, cancellationToken);

        if (existingEntity is null)
        {
            var refreshTokenExpirationData = DateTime.UtcNow.AddDays(options.RefreshTokenExpirationDays);
            var jti = Guid.CreateVersion7();
            var refreshTokenClaims = new List<Claim>()
            {
                new(AppClaimTypes.Subject, device.UserId.ToString()),
                new(AppClaimTypes.Jti, jti.ToString())
            };
            
            var refreshToken = Generate(refreshTokenClaims, refreshTokenExpirationData);
            
            var entity = new RefreshTokenEntity()
            {
                Id = jti,
                DeviceId = device.Id,
                Token = refreshToken,
                ExpireDate = refreshTokenExpirationData,
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            };

            await context.RefreshTokens.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        return accessToken;
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