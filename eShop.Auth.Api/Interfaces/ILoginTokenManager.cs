namespace eShop.Auth.Api.Interfaces;

public interface ILoginTokenManager
{
    public ValueTask<string> GenerateAsync(UserEntity user, ProviderEntity provider, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(UserEntity user, ProviderEntity provider, string token, CancellationToken cancellationToken = default);
}