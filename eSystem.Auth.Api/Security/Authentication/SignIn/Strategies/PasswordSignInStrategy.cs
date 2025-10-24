using eSystem.Application.Common.Http;
using eSystem.Auth.Api.Entities;
using eSystem.Auth.Api.Interfaces;
using eSystem.Auth.Api.Security.Identity.Options;
using eSystem.Domain.Common.Results;
using eSystem.Domain.Responses.Auth;
using eSystem.Domain.Security.Authentication;
using eSystem.Domain.Security.Lockout;

namespace eSystem.Auth.Api.Security.Authentication.SignIn.Strategies;

public sealed class PasswordSignInStrategy(
    IUserManager userManager,
    ILockoutManager lockoutManager,
    IDeviceManager deviceManager,
    IAuthorizationManager authorizationManager,
    IHttpContextAccessor accessor,
    IOptions<SignInOptions> options) : SignInStrategy
{
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;
    private readonly HttpContext httpContext = accessor.HttpContext!;
    private readonly SignInOptions options = options.Value;

    public override async ValueTask<Result> SignInAsync(Dictionary<string, object> credentials,
        CancellationToken cancellationToken = default)
    {
        var login = credentials["Login"].ToString();
        var password = credentials["Password"].ToString();

        if (string.IsNullOrEmpty(login)) return eSystem.Domain.Common.Results.Results.BadRequest("Empty login");
        if (string.IsNullOrEmpty(password)) return eSystem.Domain.Common.Results.Results.BadRequest("Empty password");

        UserEntity? user = null;
        SignInResponse? response;

        if (options.AllowUserNameLogin)
        {
            user = await userManager.FindByUsernameAsync(login, cancellationToken);
        }

        if (user is null && options.AllowEmailLogin)
        {
            user = await userManager.FindByEmailAsync(login, cancellationToken);
        }

        if (user is null) return eSystem.Domain.Common.Results.Results.NotFound($"Cannot find user with login {password}.");
        if (!user.HasPassword()) return eSystem.Domain.Common.Results.Results.BadRequest("Cannot log in, you don't have a password.");

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

            return eSystem.Domain.Common.Results.Results.BadRequest("User's primary email is not verified.", response);
        }

        if (user.LockoutState.Enabled)
        {
            response = new SignInResponse()
            {
                UserId = user.Id,
                IsLockedOut = true,
                Type = user.LockoutState.Type,
            };

            return eSystem.Domain.Common.Results.Results.BadRequest("Account is locked out", response);
        }

        if (!user.HasPassword()) return eSystem.Domain.Common.Results.Results.BadRequest("User doesn't have a password.");

        var isValidPassword = userManager.CheckPassword(user, password);
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

                return eSystem.Domain.Common.Results.Results.BadRequest("The password is not valid.", response);
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

            return eSystem.Domain.Common.Results.Results.BadRequest("Account is locked out due to too many failed login attempts", response);
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
                return eSystem.Domain.Common.Results.Results.BadRequest("Cannot sign in, device is blocked.", response);
            }

            if (!device.IsTrusted)
            {
                return eSystem.Domain.Common.Results.Results.BadRequest("You need to trust this device before sign in.", response);
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
        
        await authorizationManager.CreateAsync(device, cancellationToken);

        return Result.Success(response);
    }
}