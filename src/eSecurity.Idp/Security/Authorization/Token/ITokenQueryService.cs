using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authorization.Token;

public interface ITokenQueryService
{
    ValueTask<bool> ExistsAsync(string token, CancellationToken cancellationToken = default);

    ValueTask<OpaqueTokenEntity?> GetByIdAsync(Guid tokenId, CancellationToken cancellationToken = default);
    ValueTask<OpaqueTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    
    ValueTask<OpaqueTokenEntity?> GetByTokenAsync(string token, OpaqueTokenType type, 
        CancellationToken cancellationToken = default);
}