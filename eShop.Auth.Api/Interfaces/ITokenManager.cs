namespace eShop.Auth.Api.Interfaces;

public interface ITokenManager
{
    public Task<string> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default);
    public Result Verify();
}