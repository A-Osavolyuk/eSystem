using eSecurity.Common.Responses;
using eSecurity.Data.Entities;
using eSecurity.Security.Authentication.Lockout;
using eSecurity.Security.Authentication.Odic.Session;
using eSecurity.Security.Authentication.TwoFactor.Authenticator;
using eSecurity.Security.Authentication.TwoFactor.Secret;
using eSecurity.Security.Authorization.Devices;
using eSecurity.Security.Cryptography.Protection;
using eSecurity.Security.Identity.Options;
using eSecurity.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using Microsoft.AspNetCore.DataProtection;

namespace eSecurity.Security.Authentication.SignIn.Strategies;

public sealed class AuthenticatorSignInPayload : SignInPayload
{
    public required Guid UserId { get; set; }
    public required string Code { get; set; }
}

public sealed class AuthenticatorSignInStrategy(
    IUserManager userManager,
    ILockoutManager lockoutManager,
    IDeviceManager deviceManager,
    ISessionManager sessionManager,
    ISecretManager secretManager,
    IHttpContextAccessor accessor,
    IDataProtectionProvider protectionProvider,
    IOptions<SignInOptions> options) : ISignInStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly ISecretManager secretManager = secretManager;
    private readonly IDataProtectionProvider protectionProvider = protectionProvider;
    private readonly HttpContext httpContext = accessor.HttpContext!;
    private readonly SignInOptions options = options.Value;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload, 
        CancellationToken cancellationToken = default)
    {
        SignInResponse? response;

        if (payload is not AuthenticatorSignInPayload authenticatorPayload)
            return Results.BadRequest("Invalid payload type");

        var user = await userManager.FindByIdAsync(authenticatorPayload.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {authenticatorPayload.UserId}.");

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
        
        var userSecret = await secretManager.FindAsync(user, cancellationToken);
        if (userSecret is null) return Results.NotFound("Not found user secret");

        var protector = protectionProvider.CreateProtector(ProtectionPurposes.Secret);
        var unprotectedSecret = protector.Unprotect(userSecret.Secret);
        var isCodeVerified = AuthenticatorUtils.VerifyCode(authenticatorPayload.Code, unprotectedSecret);

        if (!isCodeVerified)
        {
            user.FailedLoginAttempts += 1;

            if (user.FailedLoginAttempts < options.MaxFailedLoginAttempts)
            {
                response = new SignInResponse()
                {
                    UserId = user.Id,
                    FailedLoginAttempts = user.FailedLoginAttempts,
                    MaxFailedLoginAttempts = options.MaxFailedLoginAttempts,
                };

                return Results.BadRequest($"Invalid two-factor code {authenticatorPayload.Code}.", response);
            }

            var deviceBlockResult = await deviceManager.BlockAsync(device, cancellationToken);
            if (!deviceBlockResult.Succeeded) return deviceBlockResult;

            var lockoutResult = await lockoutManager.BlockPermanentlyAsync(user, 
                LockoutType.TooManyFailedLoginAttempts, cancellationToken: cancellationToken);

            if (!lockoutResult.Succeeded) return lockoutResult;

            response = new SignInResponse()
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