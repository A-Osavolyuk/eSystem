using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Session;

public interface ISessionCookieFactory
{
    public string CreateCookie(SessionEntity session);
}