using eSystem.Core.Security.Authorization.OAuth.Token;
using TokenContext = eSecurity.Server.Security.Authorization.Token.Strategies.TokenContext;

namespace eSecurity.Server.Security.Authorization.Token;

public interface ITokenContextFactory
{
    public TokenContext? CreateContext(TokenRequest request);
}