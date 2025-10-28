using eSystem.Core.Requests.Auth;

namespace eSystem.Auth.Api.Security.Authentication.SSO.Token;

public abstract class TokenStrategy
{
    public abstract ValueTask<Result> HandleAsync(TokenRequest request, CancellationToken cancellationToken = default);
}