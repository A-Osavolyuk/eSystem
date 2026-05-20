using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.TokenExchange.Delegation;

public interface ITokenDelegationHandler
{
    public ValueTask<Result> HandleAsync(TokenExchangeFlowContext context, 
        CancellationToken cancellationToken = default);
}