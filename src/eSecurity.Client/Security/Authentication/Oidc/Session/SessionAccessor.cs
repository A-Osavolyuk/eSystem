using System.Text.Json;
using eSecurity.Client.Common.State.States;
using eSecurity.Core.Security.Cookies;
using eSecurity.Core.Security.Cookies.Constants;
using eSecurity.Core.Security.Cryptography.Protection.Constants;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Client.Security.Authentication.Oidc.Session;

public class SessionAccessor(
    SessionState sessionState,
    IDataProtectionProvider protectionProvider,
    IHttpContextAccessor httpContextAccessor) : ISessionAccessor
{
    private readonly SessionState _sessionState = sessionState;
    private readonly IDataProtector _protector = protectionProvider.CreateProtector(ProtectionPurposes.Session);
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    
    public SessionCookie? Get()
    {
        var cookie = _httpContext.Request.Cookies[DefaultCookies.Session];
        if (string.IsNullOrEmpty(cookie)) return _sessionState.Session;
        
        var unprotectedCookie = _protector.Unprotect(cookie);
        return JsonSerializer.Deserialize<SessionCookie>(unprotectedCookie);
    }
}