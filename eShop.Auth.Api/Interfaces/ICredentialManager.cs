namespace eShop.Auth.Api.Interfaces;

public interface ICredentialManager
{
    public ValueTask<UserCredentialEntity?> FindByCredentialIdAsync(string credentialId, CancellationToken cancellationToken);
    public ValueTask<UserCredentialEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    public ValueTask<Result> CreateAsync(UserCredentialEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(UserCredentialEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(UserCredentialEntity entity, CancellationToken cancellationToken = default);
}