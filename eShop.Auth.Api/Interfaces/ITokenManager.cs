using System.Security.Claims;
using eShop.Domain.Common.Results;

namespace eShop.Auth.Api.Interfaces;

public interface ITokenManager
{
    public Task<Result> SaveAsync(UserDeviceEntity device, 
        string refreshToken, CancellationToken cancellationToken = default);
    public Task<RefreshTokenEntity?> FindAsync(string token, CancellationToken cancellationToken = default);
    public Task<Result> RemoveAsync(RefreshTokenEntity token, CancellationToken cancellationToken = default);
    
    public string GenerateAccessToken(UserEntity user);
    public string GenerateRefreshToken(int length = 50);
}