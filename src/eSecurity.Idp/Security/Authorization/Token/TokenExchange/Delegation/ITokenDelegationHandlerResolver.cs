using eSecurity.Idp.Security.Authorization.Constants;

namespace eSecurity.Idp.Security.Authorization.Token.TokenExchange.Delegation;

public interface ITokenDelegationHandlerResolver
{
    public ITokenDelegationHandler Resolve(TokenKind kind);
}