using eSystem.Core.Primitives;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Token;

public interface ITokenValidator
{
    public ValueTask<Result> ValidateAsync(string token, CancellationToken cancellationToken = default);
}