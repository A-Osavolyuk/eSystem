using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.OAuth.Token.TokenExchange.Transformation;

public interface ITokenTransformationHandler
{
    public ValueTask<Result> HandleAsync(TokenExchangeFlowContext context, 
        CancellationToken cancellationToken = default);
}