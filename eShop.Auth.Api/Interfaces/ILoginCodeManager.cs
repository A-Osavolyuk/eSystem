namespace eShop.Auth.Api.Interfaces;

public interface ILoginCodeManager
{
    public ValueTask<string> GenerateAsync(UserEntity user, ProviderEntity provider, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(UserEntity user, ProviderEntity provider, string code, CancellationToken cancellationToken = default);
}