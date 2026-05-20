using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.TokenExchange;

public interface ITokenExchangeFlow
{
    public ValueTask<Result> ExecuteAsync(TokenExchangeFlowContext context, 
        CancellationToken cancellationToken = default);
}