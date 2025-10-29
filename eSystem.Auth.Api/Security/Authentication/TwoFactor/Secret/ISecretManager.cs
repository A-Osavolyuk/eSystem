using eSystem.Auth.Api.Data.Entities;

namespace eSystem.Auth.Api.Security.Authentication.TwoFactor.Secret;

public interface ISecretManager
{
    public ValueTask<UserSecretEntity?> FindAsync (UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> AddAsync(UserSecretEntity secret, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(UserSecretEntity secret, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(UserEntity user, CancellationToken cancellationToken = default);
    
    public string Generate();
    public string Protect(string unprotectedSecret);
    public string Unprotect(string protectedSecret);
}