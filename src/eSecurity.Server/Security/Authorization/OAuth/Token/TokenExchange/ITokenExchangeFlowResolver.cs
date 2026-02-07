namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange;

public interface ITokenExchangeFlowResolver
{
    public ITokenExchangeFlow Resolve(TokenExchangeFlow flow);
}