namespace eSecurity.Client.BFF.Security.Authentication.Token;

public interface ITokenHandler
{
    ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}