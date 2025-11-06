using eSecurity.Common.Responses;
using eSecurity.Data.Entities;
using eSecurity.Security.Authentication.Lockout;
using eSecurity.Security.Authentication.Odic.Session;
using eSecurity.Security.Authentication.Password;
using eSecurity.Security.Authorization.Devices;
using eSecurity.Security.Identity.Options;
using eSecurity.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Security.Authentication.Lockout;
using eSystem.Core.Security.Authentication.SignIn;
using eSystem.Core.Security.Authentication.SignIn.Payloads;
using eSystem.Core.Security.Identity.Email;

namespace eSecurity.Security.Authentication.SignIn.Strategies;

public sealed class PasswordSignInStrategy(
    IUserManager userManager,
    IPasswordManager passwordManager,
    ILockoutManager lockoutManager,
    IDeviceManager deviceManager,
    ISessionManager sessionManager,
    IHttpContextAccessor accessor,
    IOptions<SignInOptions> options) : SignInStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly IPasswordManager passwordManager = passwordManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly HttpContext httpContext = accessor.HttpContext!;
    private readonly SignInOptions options = options.Value;

    public override async ValueTask<Result> SignInAsync(SignInPayload payload,
        CancellationToken cancellationToken = default)
    {
        UserEntity? user = null;
        SignInResponse? response;
        
        if(payload is not PasswordSignInPayload passwordPayload)
            return Results.BadRequest("Invalid payload type");

        if (options.AllowUserNameLogin)
        {
            user = await userManager.FindByUsernameAsync(passwordPayload.Login, cancellationToken);
        }

        if (user is null && options.AllowEmailLogin)
        {
            user = await userManager.FindByEmailAsync(passwordPayload.Login, cancellationToken);
        }

        if (user is null) return Results.NotFound($"Cannot find user with login {passwordPayload.Login}.");
        if (!user.HasPassword()) return Results.BadRequest("Cannot log in, you don't have a password.");

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

        var userPrimaryEmail = user.GetEmail(EmailType.Primary)!;
        if (options.RequireConfirmedEmail && !userPrimaryEmail.IsVerified)
        {
            response = new SignInResponse()
            {
                UserId = user.Id,
                IsEmailConfirmed = false
            };

            return Results.BadRequest("User's primary email is not verified.", response);
        }

        if (user.LockoutState.Enabled)
        {
            response = new SignInResponse()
            {
                UserId = user.Id,
                IsLockedOut = true,
                Type = user.LockoutState.Type,
            };

            return Results.BadRequest("Account is locked out", response);
        }

        if (!user.HasPassword()) return Results.BadRequest("User doesn't have a password.");

        var isValidPassword = passwordManager.Check(user, passwordPayload.Password);
        if (!isValidPassword)
        {
            user.FailedLoginAttempts += 1;

            var updateResult = await userManager.UpdateAsync(user, cancellationToken);
            if (!updateResult.Succeeded) return updateResult;

            if (user.FailedLoginAttempts < options.MaxFailedLoginAttempts)
            {
                response = new SignInResponse()
                {
                    UserId = user.Id,
                    FailedLoginAttempts = user.FailedLoginAttempts,
                    MaxFailedLoginAttempts = options.MaxFailedLoginAttempts,
                };

                return Results.BadRequest("The password is not valid.", response);
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

        if (options.RequireTrustedDevice)
        {
            response = new SignInResponse()
            {
                UserId = user.Id,
                DeviceId = device.Id,
                IsDeviceTrusted = device.IsTrusted,
                IsDeviceBlocked = device.IsBlocked,
            };

            if (device.IsBlocked)
            {
                return Results.BadRequest("Cannot sign in, device is blocked.", response);
            }

            if (!device.IsTrusted)
            {
                return Results.BadRequest("You need to trust this device before sign in.", response);
            }
        }

        if (user.HasMethods() && user.TwoFactorEnabled)
        {
            response = new SignInResponse()
            {
                UserId = user.Id,
                IsTwoFactorEnabled = true,
            };

            return Result.Success(response);
        }

        response = new SignInResponse() { UserId = user.Id, };
        
        await sessionManager.CreateAsync(device, cancellationToken);

        return Result.Success(response);
    }
}