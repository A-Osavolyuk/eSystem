using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Token.TokenExchange;

public interface ITokenExchangeFlow
{
    public ValueTask<Result> ExecuteAsync(TokenExchangeFlowContext context, 
        CancellationToken cancellationToken = default);
}