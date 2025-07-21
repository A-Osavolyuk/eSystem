namespace eShop.Auth.Api.Interfaces;

public interface ISecretManager
{
    public ValueTask<UserSecretEntity?> FindAsync (UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<UserSecretEntity> GenerateAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync (UserEntity user, CancellationToken cancellationToken = default);
}