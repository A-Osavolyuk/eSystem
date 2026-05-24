namespace eSecurity.Idp.Security.Authorization.Token.TokenExchange;

public interface ITokenExchangeFlowResolver
{
    public ITokenExchangeFlow Resolve(TokenExchangeFlow flow);
}