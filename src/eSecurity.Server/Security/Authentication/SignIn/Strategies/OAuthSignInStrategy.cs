using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Authentication.SignIn.Session;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authentication.SignIn.Session;
using eSecurity.Server.Security.Authentication.TwoFactor;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Http.Results;
using eSystem.Core.Utilities.Query;
using OAuthFlow = eSecurity.Core.Security.Authorization.OAuth.OAuthFlow;

namespace eSecurity.Server.Security.Authentication.SignIn.Strategies;

public sealed class OAuthSignInStrategy(
    IUserManager userManager,
    IDeviceManager deviceManager,
    ILockoutManager lockoutManager,
    IHttpContextAccessor httpContextAccessor,
    ILinkedAccountManager linkedAccountManager,
    ISignInSessionManager signInSessionManager,
    ITwoFactorManager twoFactorManager,
    ISessionManager sessionManager) : ISignInStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly ILinkedAccountManager _linkedAccountManager = linkedAccountManager;
    private readonly ISignInSessionManager _signInSessionManager = signInSessionManager;
    private readonly ITwoFactorManager _twoFactorManager = twoFactorManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload,
        CancellationToken cancellationToken = default)
    {
        if (payload is not OAuthSignInPayload oauthPayload)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidPayloadType,
                Description = "Invalid payload"
            });
        }

        var user = await _userManager.FindByEmailAsync(oauthPayload.Email, cancellationToken);
        if (user is null) return Results.BadRequest("User not found.");

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null)
        {
            var clientInfo = _httpContext.GetClientInfo();
            device = new UserDeviceEntity
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                UserAgent = userAgent,
                IpAddress = ipAddress,
                Browser = clientInfo.UA.ToString(),
                Os = clientInfo.OS.ToString(),
                Device = clientInfo.Device.ToString(),
                IsTrusted = false,
                IsBlocked = false,
                FirstSeen = DateTimeOffset.UtcNow
            };

            var result = await _deviceManager.CreateAsync(device, cancellationToken);
            if (!result.Succeeded) return result;
        }

        if (device.IsBlocked)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.BlockedDevice,
                Description = "Device is blocked"
            });
        }

        if (!device.IsTrusted)
        {
            var deviceResult = await _deviceManager.TrustAsync(device, cancellationToken);
            if (!deviceResult.Succeeded) return deviceResult;
        }

        var lockoutState = await _lockoutManager.GetAsync(user, cancellationToken);
        if (lockoutState is null) return Results.NotFound("State not found");

        if (lockoutState.Enabled)
        {
            var lockoutResult = await _lockoutManager.UnblockAsync(user, cancellationToken);
            if (!lockoutResult.Succeeded) return lockoutResult;
        }
        
        var linkedAccount = await _linkedAccountManager.GetAsync(user, oauthPayload.LinkedAccount, cancellationToken);
        if (linkedAccount is null)
        {
            linkedAccount = new UserLinkedAccountEntity
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                Type = oauthPayload.LinkedAccount
            };
            
            var connectResult = await _linkedAccountManager.CreateAsync(linkedAccount, cancellationToken);
            if (!connectResult.Succeeded) return connectResult;
        }
        
        var session = await _signInSessionManager.FindByIdAsync(oauthPayload.SessionId, cancellationToken);
        if (session is null) return Results.BadRequest("Session not found.");

        if (session.IsActive)
        {
            session.UserId = user.Id;
            session.OAuthFlow = OAuthFlow.SignIn;
            session.CompletedSteps = [SignInStep.OAuth];

            if (await _twoFactorManager.IsEnabledAsync(user, cancellationToken))
            {
                session.RequiredSteps = [..session.RequiredSteps, SignInStep.TwoFactor];
                session.CurrentStep = SignInStep.TwoFactor;
            }
            else
            {
                session.CurrentStep = SignInStep.Complete;
                session.Status = SignInStatus.Completed;
            }
        }
        else
        {
            session.Status = SignInStatus.Expired;
        }

        var updateResult = await _signInSessionManager.UpdateAsync(session, cancellationToken);
        if (!updateResult.Succeeded) return updateResult;

        await _sessionManager.CreateAsync(device, cancellationToken);
        return Results.Found(QueryBuilder.Create().WithUri(oauthPayload.ReturnUri)
            .WithQueryParam("sid", session.Id.ToString())
            .WithQueryParam("state", oauthPayload.State)
            .Build());
    }
}