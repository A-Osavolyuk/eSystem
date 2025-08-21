namespace eShop.Auth.Api.Interfaces;

public interface ICredentialManager
{
    public ValueTask<UserCredentialEntity?> FindAsync(string credentialId, CancellationToken cancellationToken);
    public ValueTask<Result> CreateAsync(UserCredentialEntity entity, CancellationToken cancellationToken = default);
}