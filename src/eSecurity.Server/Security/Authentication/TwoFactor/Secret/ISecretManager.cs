using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.TwoFactor.Secret;

public interface ISecretManager
{
    public ValueTask<UserSecretEntity?> GetAsync (UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> AddAsync(UserSecretEntity secret, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(UserSecretEntity secret, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(UserEntity user, CancellationToken cancellationToken = default);
    public string Generate();
}