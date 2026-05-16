using eSystem.Core.Server.Security.Authorization.OAuth;

namespace eSecurity.Server.Security.Authorization.OAuth.Token;

public interface ITokenStrategyResolver
{
    public ITokenStrategy Resolve(GrantType grantType);
}