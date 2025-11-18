using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Authorization.OAuth;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Authorization.OAuth;
using eSecurity.Server.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Server.Security.Authentication.SignIn.Strategies;

public sealed class OAuthSignInStrategy(
    IUserManager userManager,
    IDeviceManager deviceManager,
    ILockoutManager lockoutManager,
    IHttpContextAccessor httpContextAccessor,
    ILinkedAccountManager providerManager,
    IOAuthSessionManager oauthSessionManager,
    ISessionManager sessionManager) : ISignInStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly ILinkedAccountManager _providerManager = providerManager;
    private readonly IOAuthSessionManager _oauthSessionManager = oauthSessionManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload,
        CancellationToken cancellationToken = default)
    {
        if(payload is not OAuthSignInPayload oauthPayload)
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.InvalidPayloadType, 
                Description = "Invalid payload type"
            });

        var user = await _userManager.FindByEmailAsync(oauthPayload.Email, cancellationToken);
        if (user is null) return Results.BadRequest($"Cannot find user with email {oauthPayload.Email}.");

        var userAgent = _httpContext.GetUserAgent();
        var ipAddress = _httpContext.GetIpV4();
        var clientInfo = _httpContext.GetClientInfo();
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
                Os = clientInfo.OS.ToString(),
                Device = clientInfo.Device.ToString(),
                IsTrusted = false,
                IsBlocked = false,
                FirstSeen = DateTimeOffset.UtcNow,
                CreateDate = DateTimeOffset.UtcNow
            };

            var result = await _deviceManager.CreateAsync(device, cancellationToken);
            if (!result.Succeeded) return result;
        }
        
        if (device.IsBlocked) 
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.BlockedDevice, 
                Description = "Device is blocked"
            });
        
        if (!device.IsTrusted)
        {
            var deviceResult = await _deviceManager.TrustAsync(device, cancellationToken);
            if (!deviceResult.Succeeded) return deviceResult;
        }

        if (user.LockoutState.Enabled)
        {
            var lockoutResult = await _lockoutManager.UnblockAsync(user, cancellationToken);
            if (!lockoutResult.Succeeded) return lockoutResult;
        }

        var linkedAccount = new UserLinkedAccountEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            Type = oauthPayload.LinkedAccount,
            CreateDate = DateTimeOffset.UtcNow,
        };

        if (!user.HasLinkedAccount(oauthPayload.LinkedAccount))
        {
            var connectResult = await _providerManager.CreateAsync(linkedAccount, cancellationToken);
            if (!connectResult.Succeeded) return connectResult;
        }

        var session = await _oauthSessionManager.FindAsync(oauthPayload.SessionId, oauthPayload.Token, cancellationToken);
        if (session is null) return Results.BadRequest($"Cannot find session with id {oauthPayload.SessionId}.");

        session.SignType = OAuthSignType.SignIn;
        session.LinkedAccountId = linkedAccount.Id;

        var updateResult = await _oauthSessionManager.UpdateAsync(session, cancellationToken);
        if (!updateResult.Succeeded) return updateResult;
        
        await _sessionManager.CreateAsync(device, cancellationToken);

        var builder = QueryBuilder.Create().WithUri(oauthPayload.ReturnUri)
            .WithQueryParam("sessionId", session.Id.ToString())
            .WithQueryParam("token", oauthPayload.Token);
        
        return Results.Ok(builder.Build());
    }
}