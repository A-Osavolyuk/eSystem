namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Logout;

public interface ILogoutStrategyResolver
{
    public ILogoutStrategy<TResult> Resolve<TResult>(LogoutFlow flow);
}