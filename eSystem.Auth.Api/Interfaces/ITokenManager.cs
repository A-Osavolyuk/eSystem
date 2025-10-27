using System.Security.Claims;

namespace eSystem.Auth.Api.Interfaces;

public interface ITokenManager
{
    public Task<Result> SaveAsync(SessionEntity session, ClientEntity client,
        string refreshToken, CancellationToken cancellationToken = default);
    public Task<RefreshTokenEntity?> FindAsync(string token, CancellationToken cancellationToken = default);
    public Task<Result> RemoveAsync(RefreshTokenEntity token, CancellationToken cancellationToken = default);
    
    public string GenerateAccessToken(IEnumerable<Claim> claims, string audience);
    public string GenerateRefreshToken(int length = 50);
}