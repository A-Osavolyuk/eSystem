namespace eShop.Auth.Api.Interfaces;

public interface ITokenManager
{
    public ValueTask<RefreshTokenEntity?> FindAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
    public Task<string> GenerateAsync(UserEntity userEntity, TokenType type, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(UserEntity userEntity, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(UserEntity userEntity, string refreshToken, CancellationToken cancellationToken = default);
}