using System.Text.Json;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Cryptography.Protection.Constants;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Session;

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