using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Strategies;

public interface ITokenStrategy
{
    public ValueTask<Result> ExecuteAsync(TokenPayload payload, CancellationToken cancellationToken = default);
}

public abstract class TokenPayload
{
    public required string ClientId { get; set; }
    public required string GrantType { get; set; }
}