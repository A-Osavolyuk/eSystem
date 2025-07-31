namespace eShop.Auth.Api.Interfaces;

public interface ITokenManager
{
    public ValueTask<RefreshTokenEntity?> FindAsync(UserEntity user, CancellationToken cancellationToken = default);
    public Task<string> GenerateAsync(UserEntity user, TokenType type, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(UserEntity user, string token, CancellationToken cancellationToken = default);
}