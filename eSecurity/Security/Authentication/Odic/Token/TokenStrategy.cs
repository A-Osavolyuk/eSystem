using eSecurity.Features.ODIC.Commands;

namespace eSecurity.Security.Authentication.Odic.Token;

public abstract class TokenStrategy
{
    public abstract ValueTask<Result> HandleAsync(TokenCommand command, CancellationToken cancellationToken = default);
}