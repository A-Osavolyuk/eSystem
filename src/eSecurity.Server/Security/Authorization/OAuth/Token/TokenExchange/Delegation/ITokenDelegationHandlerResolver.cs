using eSecurity.Server.Security.Authorization.Constants;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.TokenExchange.Delegation;

public interface ITokenDelegationHandlerResolver
{
    public ITokenDelegationHandler Resolve(TokenKind kind);
}