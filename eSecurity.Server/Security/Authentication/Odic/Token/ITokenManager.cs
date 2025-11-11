using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Odic.Token;

public interface ITokenManager
{
    public Task<RefreshTokenEntity?> FindByTokenAsync(string token, 
        CancellationToken cancellationToken = default);
    public Task<Result> CreateAsync(RefreshTokenEntity token, 
        CancellationToken cancellationToken = default);
    public Task<Result> RevokeAsync(RefreshTokenEntity revokedToken,
        CancellationToken cancellationToken = default);
    public Task<Result> RemoveAsync(RefreshTokenEntity token, 
        CancellationToken cancellationToken = default);
}