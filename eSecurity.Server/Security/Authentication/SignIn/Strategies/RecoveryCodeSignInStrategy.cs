using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.Oidc.Session;
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
    private readonly IUserManager _userManager = userManager;
    private readonly IRecoverManager _recoveryManager = recoveryManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;
    private readonly SignInOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload, CancellationToken cancellationToken = default)
    {
        if (payload is not RecoveryCodeSignInPayload recoveryPayload)
            throw new NotSupportedException("Payload type must be 'RecoveryCodeSignInPayload'");

        var user = await _userManager.FindByIdAsync(recoveryPayload.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {recoveryPayload.UserId}");

        var userAgent = _httpContext.GetUserAgent()!;
        var ipAddress = _httpContext.GetIpV4()!;
        var clientInfo = _httpContext.GetClientInfo()!;
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

        var codeResult = await _recoveryManager.VerifyAsync(user, recoveryPayload.Code, cancellationToken);
        if (!codeResult.Succeeded)
        {
            user.FailedLoginAttempts += 1;

            if (user.FailedLoginAttempts < _options.MaxFailedLoginAttempts)
                return Results.BadRequest(new Error()
                {
                    Code = Errors.Common.FailedLoginAttempt, 
                    Description = "Invalid recovery code.",
                    Details = new()
                    {
                        { "maxFailedLoginAttempts", _options.MaxFailedLoginAttempts },
                        { "failedLoginAttempts", user.FailedLoginAttempts },
                    }
                });

            var deviceBlockResult = await _deviceManager.BlockAsync(device, cancellationToken);
            if (!deviceBlockResult.Succeeded) return deviceBlockResult;

            var lockoutResult = await _lockoutManager.BlockPermanentlyAsync(user,
                LockoutType.TooManyFailedLoginAttempts, cancellationToken: cancellationToken);

            if (!lockoutResult.Succeeded) return lockoutResult;
            return Results.BadRequest(new Error()
            {
                Code = Errors.Common.AccountLockedOut,
                Description = "Account is locked out due to too many failed login attempts",
                Details = new() { { "userId", user.Id } }
            });
        }

        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;

            var userUpdateResult = await _userManager.UpdateAsync(user, cancellationToken);
            if (!userUpdateResult.Succeeded) return userUpdateResult;
        }

        var response = new SignInResponse() { UserId = user.Id, };

        await _sessionManager.CreateAsync(device, cancellationToken);

        return Results.Ok(response);
    }
}