using System.Text.Json;
using eSecurity.Common.Responses;
using eSecurity.Common.Routing;
using eSecurity.Security.Authentication.Odic.Session;
using eSecurity.Security.Cookies;
using eSecurity.Security.Cryptography.Protection;
using eSecurity.Security.Identity.User;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Security.Authentication.Odic.Logout.Strategies;

public sealed class ManualLogoutPayload : LogoutPayload;

public sealed class ManualLogoutStrategy(
    IDataProtectionProvider protectionProvider,
    IUserManager userManager,
    ISessionManager sessionManager,
    ICookieAccessor cookieAccessor) : ILogoutStrategy
{
    private readonly IDataProtectionProvider protectionProvider = protectionProvider;
    private readonly IUserManager userManager = userManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly ICookieAccessor cookieAccessor = cookieAccessor;

    public async ValueTask<Result> ExecuteAsync(LogoutPayload payload, CancellationToken cancellationToken = default)
    {
        var protectedCookie = cookieAccessor.Get(DefaultCookies.Session);
        if (string.IsNullOrEmpty(protectedCookie)) return Results.BadRequest("Session doesn't exist.");

        var protector = protectionProvider.CreateProtector(ProtectionPurposes.Session);
        var cookiesJson = protector.Unprotect(protectedCookie);
        var cookie = JsonSerializer.Deserialize<SessionCookie>(cookiesJson)!;

        var session = await sessionManager.FindByIdAsync(cookie.SessionId, cancellationToken);
        if (session is null) return Results.NotFound("Session was not found.");

        var user = await userManager.FindByIdAsync(cookie.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User was not found");
        
        var result = await sessionManager.RemoveAsync(session, cancellationToken);
        if (!result.Succeeded) return result;

        var response = new LogoutResponse() { RedirectUri = Links.Account.SignIn };
        return Result.Success(response);
    }
}