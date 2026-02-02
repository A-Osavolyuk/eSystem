namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Logout;

public interface ILogoutStrategyResolver
{
    public ILogoutStrategy<TResult> Resolve<TResult>(LogoutFlow flow) where TResult : class, new();
}