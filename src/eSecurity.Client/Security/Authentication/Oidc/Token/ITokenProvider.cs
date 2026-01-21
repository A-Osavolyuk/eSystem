namespace eSecurity.Client.Security.Authentication.Oidc.Token;

public interface ITokenProvider
{
    public string? Get(string key);
    public Task SetAsync(AuthenticationMetadata metadata, CancellationToken cancellationToken = default);
}