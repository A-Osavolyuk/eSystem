using eSecurity.Data.Entities;
using eSecurity.Security.Authentication.Lockout;
using eSecurity.Security.Authentication.Odic.Session;
using eSecurity.Security.Authorization.Devices;
using eSecurity.Security.Authorization.OAuth;
using eSecurity.Security.Authorization.OAuth.LinkedAccount;
using eSecurity.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Utilities.Query;

namespace eSecurity.Security.Authentication.SignIn.Strategies;

public sealed class OAuthSignInPayload : SignInPayload
{
    public required Guid SessionId { get; set; }
    public required string Email { get; set; }
    public required string ReturnUri { get; set; }
    public required string Token { get; set; }
    public required LinkedAccountType LinkedAccount { get; set; }
}

public sealed class OAuthSignInStrategy(
    IUserManager userManager,
    IDeviceManager deviceManager,
    ILockoutManager lockoutManager,
    IHttpContextAccessor httpContextAccessor,
    ILinkedAccountManager providerManager,
    IOAuthSessionManager oauthSessionManager,
    ISessionManager sessionManager) : ISignInStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly ILinkedAccountManager providerManager = providerManager;
    private readonly IOAuthSessionManager oauthSessionManager = oauthSessionManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload,
        CancellationToken cancellationToken = default)
    {
        if(payload is not OAuthSignInPayload oauthPayload)
            return Results.BadRequest("Invalid payload type");

        var user = await userManager.FindByEmailAsync(oauthPayload.Email, cancellationToken);
        if (user is null) return Results.BadRequest($"Cannot find user with email {oauthPayload.Email}.");

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
        
        if (device.IsBlocked) return Results.BadRequest("Device is blocked");
        if (!device.IsTrusted)
        {
            var deviceResult = await deviceManager.TrustAsync(device, cancellationToken);
            if (!deviceResult.Succeeded) return Result.Failure(deviceResult.GetError());
        }

        if (user.LockoutState.Enabled)
        {
            var lockoutResult = await lockoutManager.UnblockAsync(user, cancellationToken);
            if (!lockoutResult.Succeeded) return Result.Failure(lockoutResult.GetError());
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
            var connectResult = await providerManager.CreateAsync(linkedAccount, cancellationToken);
            if (!connectResult.Succeeded) return Result.Failure(connectResult.GetError());
        }

        var session = await oauthSessionManager.FindAsync(oauthPayload.SessionId, oauthPayload.Token, cancellationToken);
        if (session is null) return Results.BadRequest($"Cannot find session with id {oauthPayload.SessionId}.");

        session.SignType = OAuthSignType.SignIn;
        session.LinkedAccountId = linkedAccount.Id;

        var updateResult = await oauthSessionManager.UpdateAsync(session, cancellationToken);
        if (!updateResult.Succeeded) return Result.Failure(updateResult.GetError());
        
        await sessionManager.CreateAsync(device, cancellationToken);

        var builder = QueryBuilder.Create().WithUri(oauthPayload.ReturnUri)
            .WithQueryParam("sessionId", session.Id.ToString())
            .WithQueryParam("token", oauthPayload.Token);
        
        return Result.Success(builder.Build());
    }
}