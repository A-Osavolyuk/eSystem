using System.Security.Claims;
using eShop.Auth.Api.Security.Cryptography;
using eShop.Auth.Api.Security.Tokens;
using eShop.Domain.Common.Security.Constants;
using Microsoft.Extensions.Options;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ITokenManager), ServiceLifetime.Scoped)]
public sealed class TokenManager(
    AuthDbContext context,
    IOptions<JwtOptions> options,
    IKeyFactory keyFactory,
    ITokenFactory tokenFactory) : ITokenManager
{
    private readonly AuthDbContext context = context;
    private readonly IKeyFactory keyFactory = keyFactory;
    private readonly ITokenFactory tokenFactory = tokenFactory;
    private readonly JwtOptions options = options.Value;

    public async Task<string> GenerateAsync(UserDeviceEntity device,
        CancellationToken cancellationToken = default)
    {
        var existingEntity = await context.RefreshTokens.FirstOrDefaultAsync(
            x => x.DeviceId == device.Id, cancellationToken);

        if (existingEntity is null)
        {
            var refreshToken = keyFactory.Create(50);

            var entity = new RefreshTokenEntity()
            {
                Id = Guid.CreateVersion7(),
                DeviceId = device.Id,
                Token = refreshToken,
                ExpireDate = DateTime.UtcNow.AddDays(options.RefreshTokenExpirationDays),
                CreateDate = DateTime.UtcNow,
                UpdateDate = null
            };

            await context.RefreshTokens.AddAsync(entity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        var claims = new List<Claim>()
        {
            new(AppClaimTypes.Subject, device.UserId.ToString()),
            new(AppClaimTypes.Jti, Guid.CreateVersion7().ToString())
        };

        var token = tokenFactory.Create(claims);
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
}