using System.Text.Json;
using eSecurity.Server.Security.Cookies;
using eSecurity.Server.Security.Cryptography.Protection.Constants;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Session;

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