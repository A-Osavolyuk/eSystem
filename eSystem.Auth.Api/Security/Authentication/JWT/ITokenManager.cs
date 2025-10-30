using System.Security.Claims;
using eSystem.Auth.Api.Data.Entities;

namespace eSystem.Auth.Api.Security.Authentication.JWT;

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