using eShop.Auth.Api.Types;

namespace eShop.Auth.Api.Interfaces;

public interface ITokenManager
{
    public Task<Token> GenerateAsync(TokenType type, UserDeviceEntity device, 
        CancellationToken cancellationToken = default);
    public Task<RefreshTokenEntity?> FindAsync(UserDeviceEntity device, CancellationToken cancellationToken = default);
    public Task<Result> RemoveAsync(RefreshTokenEntity token, CancellationToken cancellationToken = default);
}