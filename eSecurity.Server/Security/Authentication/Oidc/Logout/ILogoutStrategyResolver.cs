using eSecurity.Core.Security.Authentication.Oidc.Logout;

namespace eSecurity.Server.Security.Authentication.Oidc.Logout;

public interface ILogoutStrategyResolver
{
    public ILogoutStrategy Resolve(LogoutType type);
}