using System.Text.Json;
using eSecurity.Client.Security.Cookies;
using eSecurity.Client.Security.Cookies.Constants;
using eSecurity.Client.Security.Cryptography.Protection.Constants;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Session;

public class SessionAccessor(
    IHttpContextAccessor httpContextAccessor,
    IDataProtectionProvider protectionProvider) : ISessionAccessor
{
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly IDataProtector _protector = protectionProvider.CreateProtector(ProtectionPurposes.Session);

    public SessionCookie? Get()
    {
        if (!_httpContext.Request.Cookies.TryGetValue(DefaultCookies.Session, out var value))
            return null;
        
        if (string.IsNullOrEmpty(value))
            return null;
        
        var json = _protector.Unprotect(value);
        return JsonSerializer.Deserialize<SessionCookie>(json);
    }
}