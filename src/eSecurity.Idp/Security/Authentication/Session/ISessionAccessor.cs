namespace eSecurity.Idp.Security.Authentication.Session;

public interface ISessionAccessor
{
    public SessionCookie? GetCookie();
}