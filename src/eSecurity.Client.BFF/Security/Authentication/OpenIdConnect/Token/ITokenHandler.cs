namespace eSecurity.Client.BFF.Security.Authentication.OpenIdConnect.Token;

public interface ITokenHandler
{
    ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}