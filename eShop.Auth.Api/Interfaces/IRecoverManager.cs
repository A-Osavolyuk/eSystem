namespace eShop.Auth.Api.Interfaces;

public interface IRecoverManager
{
    public List<string> Unprotect(UserEntity user);
    public ValueTask<List<string>> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> VerifyAsync(UserEntity user, string code, CancellationToken cancellationToken = default);
    public ValueTask<Result> RevokeAsync(UserEntity user, CancellationToken cancellationToken = default);
}