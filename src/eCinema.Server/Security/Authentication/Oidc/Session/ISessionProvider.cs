using eCinema.Server.Security.Cookies;

namespace eCinema.Server.Security.Authentication.Oidc.Session;

public interface ISessionProvider
{
    public SessionCookie? Get();
    public void Set(SessionCookie session);
}