using eSystem.Core.Requests.Auth;

namespace eSecurity.Security.Authentication.Odic.Token;

public abstract class TokenStrategy
{
    public abstract ValueTask<Result> HandleAsync(TokenRequest request, CancellationToken cancellationToken = default);
}