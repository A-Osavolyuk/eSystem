using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Oidc.Token;

public interface ITokenManager
{
    public Task<bool> IsOpaqueAsync(string token,
        CancellationToken cancellationToken = default);
    
    public Task<OpaqueTokenEntity?> FindByHashAsync(string hash,
        CancellationToken cancellationToken = default);
    
    public Task<OpaqueTokenEntity?> FindByHashAsync(string hash, OpaqueTokenType type,
        CancellationToken cancellationToken = default);

    public Task<Result> CreateAsync(OpaqueTokenEntity token, IEnumerable<ScopeEntity> scopes,
        CancellationToken cancellationToken = default);

    public Task<Result> RevokeAsync(OpaqueTokenEntity token,
        CancellationToken cancellationToken = default);

    public Task<Result> RemoveAsync(OpaqueTokenEntity token,
        CancellationToken cancellationToken = default);
}