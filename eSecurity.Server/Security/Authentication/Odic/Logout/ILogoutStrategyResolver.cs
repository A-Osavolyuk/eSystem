using eSecurity.Core.Security.Authentication.Odic.Logout;

namespace eSecurity.Server.Security.Authentication.Odic.Logout;

public interface ILogoutStrategyResolver
{
    public ILogoutStrategy Resolve(LogoutType type);
}