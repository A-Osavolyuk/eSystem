using System.Text.Json;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Common.Routing;
using eSecurity.Core.Security.Cookies;
using eSecurity.Core.Security.Cookies.Constants;
using eSecurity.Core.Security.Cryptography.Protection.Constants;
using eSecurity.Server.Security.Authentication.Odic.Session;
using eSecurity.Server.Security.Identity.User;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Server.Security.Authentication.Odic.Logout.Strategies;

public sealed class ManualLogoutPayload : LogoutPayload;

public sealed class ManualLogoutStrategy(
    IDataProtectionProvider protectionProvider,
    IUserManager userManager,
    ISessionManager sessionManager,
    ICookieAccessor cookieAccessor) : ILogoutStrategy
{
    private readonly IDataProtectionProvider _protectionProvider = protectionProvider;
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ICookieAccessor _cookieAccessor = cookieAccessor;

    public async ValueTask<Result> ExecuteAsync(LogoutPayload payload, CancellationToken cancellationToken = default)
    {
        var protectedCookie = _cookieAccessor.Get(DefaultCookies.Session);
        if (string.IsNullOrEmpty(protectedCookie)) return Results.BadRequest("Session doesn't exist.");

        var protector = _protectionProvider.CreateProtector(ProtectionPurposes.Session);
        var cookiesJson = protector.Unprotect(protectedCookie);
        var cookie = JsonSerializer.Deserialize<SessionCookie>(cookiesJson)!;

        var session = await _sessionManager.FindByIdAsync(cookie.SessionId, cancellationToken);
        if (session is null) return Results.NotFound("Session was not found.");

        var user = await _userManager.FindByIdAsync(cookie.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User was not found");
        
        var result = await _sessionManager.RemoveAsync(session, cancellationToken);
        if (!result.Succeeded) return result;

        var response = new LogoutResponse() { RedirectUri = Links.Account.SignIn };
        return Result.Success(response);
    }
}