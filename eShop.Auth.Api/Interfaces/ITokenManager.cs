namespace eShop.Auth.Api.Interfaces;

public interface ITokenManager
{
    public ValueTask<SecurityTokenEntity?> FindAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
    public Task<Token> GenerateAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
    public Task<string> RefreshAsync(UserEntity userEntity, string token, CancellationToken cancellationToken = default);
}