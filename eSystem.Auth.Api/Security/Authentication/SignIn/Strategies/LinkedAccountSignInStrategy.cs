using eSystem.Application.Common.Http;
using eSystem.Auth.Api.Entities;
using eSystem.Auth.Api.Interfaces;
using eSystem.Auth.Api.Utilities;
using eSystem.Domain.Common.Results;
using eSystem.Domain.Security.Authentication;
using eSystem.Domain.Security.Authorization.OAuth;

namespace eSystem.Auth.Api.Security.Authentication.SignIn.Strategies;

public class LinkedAccountSignInStrategy(
    IUserManager userManager,
    IDeviceManager deviceManager,
    ILockoutManager lockoutManager,
    IHttpContextAccessor httpContextAccessor,
    ILinkedAccountManager providerManager,
    IOAuthSessionManager sessionManager,
    ILoginManager loginManager,
    IAuthorizationManager authorizationManager) : SignInStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly ILinkedAccountManager providerManager = providerManager;
    private readonly IOAuthSessionManager sessionManager = sessionManager;
    private readonly ILoginManager loginManager = loginManager;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public override async ValueTask<Result> SignInAsync(Dictionary<string, object> credentials,
        CancellationToken cancellationToken = default)
    {
        var email = credentials["Email"] as string;
        var fallbackUri = (credentials["FallbackUri"] as string)!;
        var returnUri = credentials["ReturnUri"] as string;
        var linkedAccountType = (LinkedAccountType)credentials["Type"];
        var sessionId = credentials["SessionId"] as string;
        var token = credentials["Token"] as string;

        if (string.IsNullOrWhiteSpace(email)) return eSystem.Domain.Common.Results.Results.BadRequest("Email is empty", fallbackUri);
        if (string.IsNullOrWhiteSpace(returnUri)) return eSystem.Domain.Common.Results.Results.BadRequest("Return URI is empty", fallbackUri);
        if (string.IsNullOrWhiteSpace(token)) return eSystem.Domain.Common.Results.Results.BadRequest("Token name is empty", fallbackUri);
        if (string.IsNullOrWhiteSpace(sessionId)) return eSystem.Domain.Common.Results.Results.BadRequest("Session ID is empty", fallbackUri);

        var user = await userManager.FindByEmailAsync(email, cancellationToken);
        if (user is null) return eSystem.Domain.Common.Results.Results.BadRequest($"Cannot find user with email {email}.");

        var userAgent = httpContext.GetUserAgent();
        var ipAddress = httpContext.GetIpV4();
        var clientInfo = httpContext.GetClientInfo();
        var device = user.GetDevice(userAgent, ipAddress);

        if (device is null)
        {
            device = new UserDeviceEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                UserAgent = userAgent,
                IpAddress = ipAddress,
                Browser = clientInfo.UA.ToString(),
                OS = clientInfo.OS.ToString(),
                Device = clientInfo.Device.ToString(),
                IsTrusted = false,
                IsBlocked = false,
                FirstSeen = DateTimeOffset.UtcNow,
                CreateDate = DateTimeOffset.UtcNow
            };

            var result = await deviceManager.CreateAsync(device, cancellationToken);
            if (!result.Succeeded) return result;
        }
        
        if (device.IsBlocked) return eSystem.Domain.Common.Results.Results.BadRequest("Device is blocked", fallbackUri);
        if (!device.IsTrusted)
        {
            var deviceResult = await deviceManager.TrustAsync(device, cancellationToken);
            if (!deviceResult.Succeeded) return Result.Failure(deviceResult.GetError(), fallbackUri);
        }

        if (user.LockoutState.Enabled)
        {
            var lockoutResult = await lockoutManager.UnblockAsync(user, cancellationToken);
            if (!lockoutResult.Succeeded) return Result.Failure(lockoutResult.GetError(), fallbackUri);
        }

        var linkedAccount = new UserLinkedAccountEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Type = linkedAccountType,
            CreateDate = DateTimeOffset.UtcNow,
        };

        if (!user.HasLinkedAccount(linkedAccountType))
        {
            var connectResult = await providerManager.CreateAsync(linkedAccount, cancellationToken);
            if (!connectResult.Succeeded) return Result.Failure(connectResult.GetError(), fallbackUri);
        }

        var session = await sessionManager.FindAsync(Guid.Parse(sessionId), token, cancellationToken);
        if (session is null) return eSystem.Domain.Common.Results.Results.BadRequest("Cannot find session with id {sessionId}.", fallbackUri);

        session.SignType = OAuthSignType.SignIn;
        session.LinkedAccountId = linkedAccount.Id;

        var updateResult = await sessionManager.UpdateAsync(session, cancellationToken);
        if (!updateResult.Succeeded) return Result.Failure(updateResult.GetError(), fallbackUri);

        await loginManager.CreateAsync(device, LoginType.OAuth, linkedAccountType.ToString(), cancellationToken);
        await authorizationManager.CreateAsync(device, cancellationToken);

        var link = UrlGenerator.Url(returnUri, new { sessionId = session.Id, token });
        return Result.Success(link);
    }
}