using eSecurity.Features.Odic.Commands;

namespace eSecurity.Security.Authentication.Odic.Token;

public interface ITokenStrategy
{
    public ValueTask<Result> ExecuteAsync(TokenPayload payload, CancellationToken cancellationToken = default);
}

public abstract class TokenPayload
{
    public required string ClientId { get; set; }
    public required string GrantType { get; set; }
}