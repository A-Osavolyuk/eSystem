using System.Text.Json;
using eSecurity.Idp.Security.Cookies;
using eSecurity.Idp.Security.Cryptography.Protection.Constants;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Idp.Security.Authentication.Session;

public sealed class SessionAccessor(
    IHttpContextAccessor httpContextAccessor,
    IDataProtectionProvider protectionProvider) : ISessionAccessor
{
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public SessionCookie? GetCookie()
    {
        if (!_httpContext.Request.Cookies.TryGetValue(DefaultCookies.Session, out var cookie) ||
            string.IsNullOrEmpty(cookie))
        {
            return null;
        }

        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Session);
        var unprotectedCookie = protector.Unprotect(cookie);
        try
        {
            return JsonSerializer.Deserialize<SessionCookie>(unprotectedCookie);
        }
        catch (Exception)
        {
            return null;
        }
    }
}