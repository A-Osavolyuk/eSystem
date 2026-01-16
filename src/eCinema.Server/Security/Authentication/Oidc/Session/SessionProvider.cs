using System.Text.Json;
using eCinema.Server.Security.Cookies;
using eCinema.Server.Security.Cryptography.Protection.Constants;
using Microsoft.AspNetCore.DataProtection;

namespace eCinema.Server.Security.Authentication.Oidc.Session;

public class SessionProvider(
    IHttpContextAccessor httpContextAccessor,
    IDataProtectionProvider protectionProvider) : ISessionProvider
{
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly IDataProtector _protector = protectionProvider.CreateProtector(ProtectionPurposes.Session);

    public SessionCookie? Get()
    {
        if (!_httpContext.Request.Cookies.TryGetValue(DefaultCookies.Session, out var cookie) 
            || string.IsNullOrEmpty(cookie)) return null;
        
        var cookieJson = _protector.Unprotect(cookie);
        return JsonSerializer.Deserialize<SessionCookie?>(cookieJson);

    }

    public void Set(SessionCookie session)
    {
        var sessionJson = JsonSerializer.Serialize(session);
        var protectedCookie = _protector.Protect(sessionJson);
        
        _httpContext.Response.Cookies.Append(DefaultCookies.Session, protectedCookie, new CookieOptions()
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Expires = session.ExpiresAt,
            MaxAge = TimeSpan.FromDays(30)
        });
    }
}