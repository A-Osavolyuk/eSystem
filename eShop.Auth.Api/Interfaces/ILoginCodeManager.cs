namespace eShop.Auth.Api.Interfaces;

public interface ILoginCodeManager
{
    public ValueTask<string> GenerateAsync(UserEntity user, TwoFactorProviderEntity twoFactorProvider, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(UserEntity user, TwoFactorProviderEntity twoFactorProvider, string code, CancellationToken cancellationToken = default);
}