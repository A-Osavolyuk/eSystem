using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.Session;

public interface ISessionCookieFactory
{
    public string CreateCookie(SessionEntity session);
}