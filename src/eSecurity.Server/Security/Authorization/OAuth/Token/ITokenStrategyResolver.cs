using eSystem.Core.Security.Authorization.OAuth;
using eSystem.Core.Security.Authorization.OAuth.Constants;

namespace eSecurity.Server.Security.Authorization.OAuth.Token;

public interface ITokenStrategyResolver
{
    public ITokenStrategy Resolve(GrantType grantType);
}