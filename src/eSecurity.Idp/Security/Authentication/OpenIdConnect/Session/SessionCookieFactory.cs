using System.Text.Json;
using eSecurity.Idp.Data.Entities;
using eSecurity.Idp.Security.Cryptography.Protection.Constants;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Idp.Security.Authentication.OpenIdConnect.Session;

public sealed class SessionCookieFactory(IDataProtectionProvider protectionProvider) : ISessionCookieFactory
{
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;

    public string CreateCookie(SessionEntity session)
    {
        var cookie = new SessionCookie { SessionId = session.Id };
        var cookieJson = JsonSerializer.Serialize(cookie);
        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Session);
        return protector.Protect(cookieJson);
    }
}