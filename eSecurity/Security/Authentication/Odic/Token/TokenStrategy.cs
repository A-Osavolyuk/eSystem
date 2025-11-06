using eSecurity.Features.ODIC.Commands;
using eSystem.Core.Requests.Auth;

namespace eSecurity.Security.Authentication.Odic.Token;

public abstract class TokenStrategy
{
    public abstract ValueTask<Result> HandleAsync(TokenCommand command, CancellationToken cancellationToken = default);
}