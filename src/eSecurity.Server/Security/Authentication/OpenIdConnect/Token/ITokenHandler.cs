namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token;

public interface ITokenHandler
{
    ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}