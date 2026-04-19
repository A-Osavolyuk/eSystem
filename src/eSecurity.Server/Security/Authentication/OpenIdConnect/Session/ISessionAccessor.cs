namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Session;

public interface ISessionAccessor
{
    public SessionCookie? GetCookie();
}