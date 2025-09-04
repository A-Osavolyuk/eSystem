using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record LoginCommand(LoginRequest Request, HttpContext Context) : IRequest<Result>;

public sealed class LoginCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    ILockoutManager lockoutManager,
    IReasonManager reasonManager,
    ILoginSessionManager loginSessionManager,
    IDeviceManager deviceManager,
    IdentityOptions identityOptions) : IRequestHandler<LoginCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IReasonManager reasonManager = reasonManager;
    private readonly ILoginSessionManager loginSessionManager = loginSessionManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        UserEntity? user = null;

        if (identityOptions.SignIn.AllowUserNameLogin)
        {
            user = await userManager.FindByUsernameAsync(request.Request.Login, cancellationToken);
        }

        if (user is null && identityOptions.SignIn.AllowEmailLogin)
        {
            user = await userManager.FindByEmailAsync(request.Request.Login, cancellationToken);
        }

        if (user is null) return Results.NotFound($"Cannot find user with login {request.Request.Login}.");
        if (!user.HasPassword()) return Results.BadRequest("Cannot log in, you don't have a password.");

        var userAgent = RequestUtils.GetUserAgent(request.Context);
        var ipAddress = RequestUtils.GetIpV4(request.Context);
        var clientInfo = RequestUtils.GetClientInfo(request.Context);

        var device = await deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);

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

        var emailResult = CheckEmail(user);
        if (!emailResult.Succeeded) return emailResult;

        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);
        if (lockoutState.Enabled) return LockedOut(user, lockoutState);

        var passwordResult = await CheckPasswordAsync(user, request.Request.Password, cancellationToken);
        if (!passwordResult.Succeeded) return passwordResult;

        if (user.FailedLoginAttempts > 0)
        {
            var resetResult = await ResetFailedLoginAttemptsAsync(user, cancellationToken);
            if (!resetResult.Succeeded) return resetResult;
        }

        if (identityOptions.SignIn.RequireTrustedDevice)
        {
            var deviceResult = CheckDevice(user, device);
            if (!deviceResult.Succeeded) return deviceResult;
        }

        if (user.HasTwoFactor()) return CheckTwoFactorState(user, lockoutState);

        var tokenResult = await GenerateTokensAsync(user, lockoutState, device, cancellationToken);
        return tokenResult;
    }

    private Result CheckDevice(UserEntity user, UserDeviceEntity device)
    {
        if (device.IsBlocked)
        {
            var response = new LoginResponse()
            {
                UserId = user.Id,
                Email = user.Email,
                IsTrustedDevice = device.IsTrusted,
                IsBlockedDevice = device.IsBlocked,
                DeviceId = device.Id
            };

            return Results.BadRequest("Cannot sign in, device is blocked.", response);
        }

        if (!device.IsTrusted)
        {
            var response = new LoginResponse()
            {
                UserId = user.Id,
                Email = user.Email,
                IsTrustedDevice = device.IsTrusted,
                IsBlockedDevice = device.IsBlocked,
                DeviceId = device.Id
            };

            return Results.BadRequest("You need to trust this device before sign in.", response);
        }

        return Result.Success();
    }

    private Result LockedOut(UserEntity user, LockoutStateEntity lockoutState)
    {
        var response = new LoginResponse()
        {
            UserId = user.Id,
            IsLockedOut = lockoutState.Enabled,
            Reason = Mapper.Map(lockoutState.Reason),
        };

        return Results.BadRequest("Account is locked out", response);
    }

    private Result CheckEmail(UserEntity user)
    {
        if (identityOptions.SignIn.RequireConfirmedEmail && !user.EmailConfirmed)
        {
            var response = new LoginResponse()
            {
                UserId = user.Id,
                Email = user.Email,
                IsEmailConfirmed = false
            };

            return Results.BadRequest("The email address is not confirmed.", response);
        }

        return Result.Success();
    }

    private async Task<Result> CheckPasswordAsync(UserEntity user, string password, CancellationToken cancellationToken)
    {
        if (!user.HasPassword()) return Results.BadRequest("Password was not provided.");

        var isValidPassword = await userManager.CheckPasswordAsync(user, password, cancellationToken);
        if (isValidPassword) return Result.Success();

        user.FailedLoginAttempts += 1;

        var updateResult = await userManager.UpdateAsync(user, cancellationToken);
        if (!updateResult.Succeeded) return updateResult;

        if (user.FailedLoginAttempts < identityOptions.SignIn.MaxFailedLoginAttempts)
            return MaxFailedLoginAttempts(user);

        var reason = await reasonManager.FindByTypeAsync(LockoutType.TooManyFailedLoginAttempts, cancellationToken);
        if (reason is null)
            return Results.NotFound($"Cannot find lockout type {LockoutType.TooManyFailedLoginAttempts}.");

        var lockoutResult = await lockoutManager.LockoutAsync(user, reason,
            permanent: true, cancellationToken: cancellationToken);

        if (!lockoutResult.Succeeded) return lockoutResult;

        return Results.BadRequest("Account is locked out due to too many failed login attempts",
            new LoginResponse()
            {
                UserId = user.Id,
                IsLockedOut = true,
                FailedLoginAttempts = user.FailedLoginAttempts,
                MaxFailedLoginAttempts = identityOptions.SignIn.MaxFailedLoginAttempts,
                Reason = Mapper.Map(reason)
            });
    }

    private Result MaxFailedLoginAttempts(UserEntity user)
    {
        var response = new LoginResponse()
        {
            FailedLoginAttempts = user.FailedLoginAttempts,
            MaxFailedLoginAttempts = identityOptions.SignIn.MaxFailedLoginAttempts,
            IsLockedOut = false,
            UserId = user.Id,
        };

        return Results.BadRequest("The password is not valid.", response);
    }

    private async Task<Result> ResetFailedLoginAttemptsAsync(UserEntity user, CancellationToken cancellationToken)
    {
        user.FailedLoginAttempts = 0;
        var result = await userManager.UpdateAsync(user, cancellationToken);
        return result;
    }

    private Result CheckTwoFactorState(UserEntity user, LockoutStateEntity lockoutState)
    {
        return Result.Success(new LoginResponse()
        {
            IsLockedOut = lockoutState.Enabled,
            IsTwoFactorEnabled = true,
            UserId = user.Id
        });
    }

    private async Task<Result> GenerateTokensAsync(UserEntity user, LockoutStateEntity lockoutState,
        UserDeviceEntity device, CancellationToken cancellationToken)
    {
        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);

        var response = new LoginResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            UserId = user.Id,
            IsTwoFactorEnabled = user.HasTwoFactor(),
            IsLockedOut = lockoutState.Enabled,
        };

        await loginSessionManager.CreateAsync(device, LoginType.Password, cancellationToken: cancellationToken);

        return Result.Success(response, "Successfully logged in.");
    }
}