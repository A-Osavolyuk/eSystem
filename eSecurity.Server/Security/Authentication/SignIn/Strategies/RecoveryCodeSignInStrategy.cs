using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.Odic.Session;
using eSecurity.Server.Security.Authentication.TwoFactor.Recovery;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Security.Authentication.SignIn.Strategies;

public sealed class RecoveryCodeSignInStrategy(
    IUserManager userManager,
    IRecoverManager recoveryManager,
    IHttpContextAccessor httpContextAccessor,
    IDeviceManager deviceManager,
    ISessionManager sessionManager,
    ILockoutManager lockoutManager,
    IOptions<SignInOptions> options) : ISignInStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly IRecoverManager recoveryManager = recoveryManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;
    private readonly SignInOptions options = options.Value;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload, CancellationToken cancellationToken = default)
    {
        SignInResponse response;

        if (payload is not RecoveryCodeSignInPayload recoveryPayload)
            throw new NotSupportedException("Payload type must be 'RecoveryCodeSignInPayload'");

        var user = await userManager.FindByIdAsync(recoveryPayload.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {recoveryPayload.UserId}");

        var userAgent = httpContext.GetUserAgent()!;
        var ipAddress = httpContext.GetIpV4()!;
        var clientInfo = httpContext.GetClientInfo()!;
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

        var codeResult = await recoveryManager.VerifyAsync(user, recoveryPayload.Code, cancellationToken);
        if (!codeResult.Succeeded)
        {
            user.FailedLoginAttempts += 1;

            if (user.FailedLoginAttempts < options.MaxFailedLoginAttempts)
            {
                response = new SignInResponse
                {
                    UserId = user.Id,
                    FailedLoginAttempts = user.FailedLoginAttempts,
                    MaxFailedLoginAttempts = options.MaxFailedLoginAttempts,
                };

                return Results.BadRequest("Invalid code.", response);
            }

            var deviceBlockResult = await deviceManager.BlockAsync(device, cancellationToken);
            if (!deviceBlockResult.Succeeded) return deviceBlockResult;

            var lockoutResult = await lockoutManager.BlockPermanentlyAsync(user,
                LockoutType.TooManyFailedLoginAttempts, cancellationToken: cancellationToken);

            if (!lockoutResult.Succeeded) return lockoutResult;

            response = new SignInResponse
            {
                UserId = user.Id,
                IsLockedOut = true,
                FailedLoginAttempts = user.FailedLoginAttempts,
                MaxFailedLoginAttempts = options.MaxFailedLoginAttempts,
                Type = LockoutType.TooManyFailedLoginAttempts
            };

            return Results.BadRequest("Account is locked out due to too many failed login attempts", response);
        }

        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;

            var userUpdateResult = await userManager.UpdateAsync(user, cancellationToken);
            if (!userUpdateResult.Succeeded) return userUpdateResult;
        }

        response = new SignInResponse() { UserId = user.Id, };

        await sessionManager.CreateAsync(device, cancellationToken);

        return Result.Success(response);
    }
}