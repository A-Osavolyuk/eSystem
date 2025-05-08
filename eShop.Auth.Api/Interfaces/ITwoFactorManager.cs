namespace eShop.Auth.Api.Interfaces;

public interface ITwoFactorManager
{
    public ValueTask<Result> EnableAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> DisableAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<string> GenerateTokenAsync(UserEntity user, string provider, CancellationToken cancellationToken = default);
    public ValueTask<bool> VerifyTokenAsync(UserEntity user, string provider, string token, CancellationToken cancellationToken = default);
}