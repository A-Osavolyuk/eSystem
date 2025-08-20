namespace eShop.Auth.Api.Interfaces;

public interface ICredentialManager
{
    public ValueTask<Result> CreateAsync(UserCredentialEntity entity, CancellationToken cancellationToken = default);
}