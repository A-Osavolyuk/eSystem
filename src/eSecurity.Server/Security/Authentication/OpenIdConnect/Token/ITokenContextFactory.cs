using eSystem.Core.Security.Authentication.OpenIdConnect.Token;
using TokenContext = eSecurity.Server.Security.Authentication.OpenIdConnect.Token.Strategies.TokenContext;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Token;

public interface ITokenContextFactory
{
    public TokenContext? CreateContext(TokenRequest request);
}