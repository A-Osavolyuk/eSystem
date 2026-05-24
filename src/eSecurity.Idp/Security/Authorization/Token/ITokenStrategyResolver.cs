using eSystem.Core.Security.Authorization.OAuth;

namespace eSecurity.Idp.Security.Authorization.Token;

public interface ITokenStrategyResolver
{
    public ITokenStrategy Resolve(GrantType grantType);
}