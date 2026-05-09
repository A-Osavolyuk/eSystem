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

    public bool Exists() => _httpContext.Request.Cookies.ContainsKey(DefaultCookies.Session);
}