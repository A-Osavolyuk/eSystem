using eShop.Domain.Common.Security.Credentials;

namespace eShop.Auth.Api.Interfaces;

public interface IPasskeyManager
{
    public ValueTask<UserPasskeyEntity?> FindByCredentialIdAsync(string credentialId, CancellationToken cancellationToken);
    public ValueTask<UserPasskeyEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    public ValueTask<Result> CreateAsync(UserPasskeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(UserPasskeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(UserPasskeyEntity entity, CancellationToken cancellationToken = default);
    public ValueTask<Result> SignInAsync(UserPasskeyEntity passkey, PublicKeyCredential credential,
        string storedChallenge, CancellationToken cancellationToken);
}