using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Delegation;

public interface ITokenDelegationHandler
{
    public ValueTask<Result> HandleAsync(TokenExchangeFlowContext context, 
        CancellationToken cancellationToken = default);
}