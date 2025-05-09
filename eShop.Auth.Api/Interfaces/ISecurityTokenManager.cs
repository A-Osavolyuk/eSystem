using SecurityToken = eShop.Domain.Types.SecurityToken;

namespace eShop.Auth.Api.Interfaces;

public interface ISecurityTokenManager
{
    public ValueTask<SecurityTokenEntity?> FindAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
    public Task<SecurityToken> GenerateAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
    public Task<SecurityToken> RefreshAsync(UserEntity userEntity, SecurityTokenEntity tokenEntity, CancellationToken cancellationToken = default);
}