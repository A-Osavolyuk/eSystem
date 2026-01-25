namespace eSecurity.Server.Security.Authentication.Handlers.Basic;

public interface IBasicAuthenticationStrategyResolver
{
    public IBasicAuthenticationStrategy Resolve(HttpContext httpContext);
}