namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange;

public interface ITokenExchangeFlow
{
    public ValueTask<Result> ExecuteAsync(TokenExchangeFlowContext context, 
        CancellationToken cancellationToken = default);
}