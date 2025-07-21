namespace eShop.Auth.Api.Interfaces;

public interface IRecoverManager
{
    public ValueTask<List<string>> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(UserEntity user, string code, CancellationToken cancellationToken = default);
}