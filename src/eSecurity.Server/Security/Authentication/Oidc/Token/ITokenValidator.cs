using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.Oidc.Token;

public interface ITokenValidator
{
    public ValueTask<Result> ValidateAsync(string token, CancellationToken cancellationToken = default);
}