namespace eSecurity.Client.Security.Authentication.Oidc.Token;

public interface ITokenProvider
{
    public Task<string?> GetAsync(string key, CancellationToken cancellationToken = default);
    public Task SetAsync(string key, string token, TimeSpan timeStamp, CancellationToken cancellationToken = default);
}