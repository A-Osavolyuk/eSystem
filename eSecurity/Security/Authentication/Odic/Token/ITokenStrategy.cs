using eSecurity.Features.Odic.Commands;

namespace eSecurity.Security.Authentication.Odic.Token;

public interface ITokenStrategy
{
    public ValueTask<Result> ExecuteAsync(TokenPayload payload, CancellationToken cancellationToken = default);
}

public abstract class TokenPayload {}