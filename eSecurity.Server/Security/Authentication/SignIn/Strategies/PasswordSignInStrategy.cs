using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Lockout;
using eSecurity.Core.Security.Authentication.SignIn;
using eSecurity.Core.Security.Identity;
using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Authentication.Password;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.Options;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Common.Http.Context;

namespace eSecurity.Server.Security.Authentication.SignIn.Strategies;

public sealed class PasswordSignInStrategy(
    IUserManager userManager,
    IPasswordManager passwordManager,
    ILockoutManager lockoutManager,
    IDeviceManager deviceManager,
    ISessionManager sessionManager,
    IHttpContextAccessor accessor,
    IOptions<SignInOptions> options) : ISignInStrategy
{
    private readonly IUserManager _userManager = userManager;
    private readonly IPasswordManager _passwordManager = passwordManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly HttpContext _httpContext = accessor.HttpContext!;
    private readonly SignInOptions _options = options.Value;

    public async ValueTask<Result> ExecuteAsync(SignInPayload payload,
        CancellationToken cancellationToken = default)
    {
        UserEntity? user = null;
        SignInResponse? response;
        
        if(payload is not PasswordSignInPayload passwordPayload)
            return Results.BadRequest("Invalid payload type");

        if (_options.AllowUserNameLogin)
        {
            user = await _userManager.FindByUsernameAsync(passwordPayload.Login, cancellationToken);
        }

        if (user is null && _options.AllowEmailLogin)
        {
            user = await _userManager.FindByEmailAsync(passwordPayload.Login, cancellationToken);
        }

        if (user is null) return Results.NotFound($"Cannot find user with login {passwordPayload.Login}.");
        if (!user.HasPassword()) return Results.BadRequest("Cannot log in, you don't have a password.");

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

        var userPrimaryEmail = user.GetEmail(EmailType.Primary)!;
        if (_options.RequireConfirmedEmail && !userPrimaryEmail.IsVerified)
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

        var isValidPassword = _passwordManager.Check(user, passwordPayload.Password);
        if (!isValidPassword)
        {
            user.FailedLoginAttempts += 1;

            var updateResult = await _userManager.UpdateAsync(user, cancellationToken);
            if (!updateResult.Succeeded) return updateResult;

            if (user.FailedLoginAttempts < _options.MaxFailedLoginAttempts)
            {
                response = new SignInResponse()
                {
                    UserId = user.Id,
                    FailedLoginAttempts = user.FailedLoginAttempts,
                    MaxFailedLoginAttempts = _options.MaxFailedLoginAttempts,
                };

                return Results.BadRequest("The password is not valid.", response);
            }

            var deviceBlockResult = await _deviceManager.BlockAsync(device, cancellationToken);
            if (!deviceBlockResult.Succeeded) return deviceBlockResult;

            var lockoutResult = await _lockoutManager.BlockPermanentlyAsync(user,
                LockoutType.TooManyFailedLoginAttempts, cancellationToken: cancellationToken);

            if (!lockoutResult.Succeeded) return lockoutResult;

            response = new SignInResponse()
            {
                UserId = user.Id,
                IsLockedOut = true,
                FailedLoginAttempts = user.FailedLoginAttempts,
                MaxFailedLoginAttempts = _options.MaxFailedLoginAttempts,
                Type = LockoutType.TooManyFailedLoginAttempts
            };

            return Results.BadRequest("Account is locked out due to too many failed login attempts", response);
        }

        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;

            var userUpdateResult = await _userManager.UpdateAsync(user, cancellationToken);
            if (!userUpdateResult.Succeeded) return userUpdateResult;
        }

        if (_options.RequireTrustedDevice)
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
        
        await _sessionManager.CreateAsync(device, cancellationToken);

        return Result.Success(response);
    }
}