using eSecurity.Core.Security.Cookies;

namespace eSecurity.Client.Security.Authentication.Oidc.Session;

public interface ISessionAccessor
{
    public SessionCookie? Get();
}