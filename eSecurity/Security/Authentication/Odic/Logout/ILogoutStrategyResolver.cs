namespace eSecurity.Security.Authentication.Odic.Logout;

public interface ILogoutStrategyResolver
{
    public ILogoutStrategy Resolve(LogoutType type);
}