using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Transformation;

public interface ITokenTransformationHandler
{
    public ValueTask<Result> HandleAsync(TokenExchangeFlowContext context, 
        CancellationToken cancellationToken = default);
}