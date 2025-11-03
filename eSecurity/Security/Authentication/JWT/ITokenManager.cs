using System.Security.Claims;
using eSecurity.Data.Entities;

namespace eSecurity.Security.Authentication.JWT;

public interface ITokenManager
{
    public Task<RefreshTokenEntity?> FindByTokenAsync(string token, 
        CancellationToken cancellationToken = default);
    public Task<Result> CreateAsync(RefreshTokenEntity token, 
        CancellationToken cancellationToken = default);
    public Task<Result> RotateAsync(RefreshTokenEntity revokedToken, 
        RefreshTokenEntity newToken, CancellationToken cancellationToken = default);
    public Task<Result> RemoveAsync(RefreshTokenEntity token, 
        CancellationToken cancellationToken = default);
}