namespace eCinema.Server.Security.Authentication.OpenIdConnect.Token;

public interface ITokenHandler
{
    public ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}