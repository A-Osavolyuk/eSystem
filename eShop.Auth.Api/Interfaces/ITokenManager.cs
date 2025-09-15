using System.Security.Claims;

namespace eShop.Auth.Api.Interfaces;

public interface ITokenManager
{
    public Task<string> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default);
    public Task<RefreshTokenEntity?> FindAsync(UserEntity user, CancellationToken cancellationToken = default);
}