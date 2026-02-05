using eSystem.Core.Security.Authorization.OAuth.Token;
using TokenContext = eSecurity.Server.Security.Authorization.OAuth.Token.TokenContext;

namespace eSecurity.Server.Security.Authorization.OAuth.Token;

public interface ITokenContextFactory
{
    public TokenContext? CreateContext(TokenRequest request);
}