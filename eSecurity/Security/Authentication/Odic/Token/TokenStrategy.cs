using eSecurity.Features.Odic.Commands;

namespace eSecurity.Security.Authentication.Odic.Token;

public abstract class TokenStrategy
{
    public abstract ValueTask<Result> HandleAsync(TokenCommand command, CancellationToken cancellationToken = default);
}