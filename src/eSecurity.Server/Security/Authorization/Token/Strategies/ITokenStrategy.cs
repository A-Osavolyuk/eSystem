using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.Token.Strategies;

public interface ITokenStrategy
{
    public ValueTask<Result> ExecuteAsync(TokenContext context, CancellationToken cancellationToken = default);
}

public abstract class TokenContext
{
    public required string ClientId { get; set; }
    public required string GrantType { get; set; }
}