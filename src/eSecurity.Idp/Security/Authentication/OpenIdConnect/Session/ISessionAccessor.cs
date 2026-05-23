namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Session;

public interface ISessionAccessor
{
    public SessionCookie? GetCookie();
}