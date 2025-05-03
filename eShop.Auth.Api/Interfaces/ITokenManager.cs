namespace eShop.Auth.Api.Interfaces;

public interface ITokenManager
{
    public ValueTask<SecurityTokenEntity?> FindAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
    public Task<Token> GenerateAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
    public Task<Token> RefreshAsync(UserEntity userEntity, SecurityTokenEntity tokenEntity, CancellationToken cancellationToken = default);
}