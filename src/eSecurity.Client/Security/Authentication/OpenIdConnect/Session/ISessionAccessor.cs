using eSecurity.Client.Security.Cookies;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Session;

public interface ISessionAccessor
{
    public SessionCookie? Get();
}