using System.Text.Json;
using eSecurity.Client.Common.State.States;
using eSecurity.Client.Security.Cookies;
using eSecurity.Client.Security.Cookies.Constants;
using eSecurity.Client.Security.Cryptography.Protection.Constants;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect.Session;

public class SessionAccessor(
    UserState userState,
    IHttpContextAccessor httpContextAccessor,
    IDataProtectionProvider protectionProvider) : ISessionAccessor
{
    private readonly UserState _userState = userState;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly IDataProtector _protector = protectionProvider.CreateProtector(ProtectionPurposes.Session);

    public SessionCookie? Get()
    {
        if (!_httpContext.Request.Cookies.TryGetValue(DefaultCookies.Session, out var value))
            return null;
        
        if (string.IsNullOrEmpty(value))
            return null;
        
        var json = _protector.Unprotect(value);
        var session = JsonSerializer.Deserialize<SessionCookie>(json);
        if (session is null) 
            return null;

        _userState.UserId = session.UserId;
        return session;
    }
}