namespace eShop.Auth.Api.Interfaces;

public interface ISecretManager
{
    public ValueTask<UserSecretEntity?> FindAsync (UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> SaveAsync(UserSecretEntity secret, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(UserEntity user, CancellationToken cancellationToken = default);
    public string Generate();
    public string Protect(string unprotectedSecret);
    public string Unprotect(string protectedSecret);
}