using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Session;

public interface ISessionCookieFactory
{
    public string CreateCookie(SessionEntity session);
}