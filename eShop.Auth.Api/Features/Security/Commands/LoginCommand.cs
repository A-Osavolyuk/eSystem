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
        LoginResponse? response;

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

        if (identityOptions.SignIn.RequireConfirmedEmail 
            && user.Emails.Any(x => x is { Type: EmailType.Primary, IsVerified: false }))
        {
            response = new LoginResponse()
            {
                UserId = user.Id,
                Email = user.Emails.First(x => x.Type is EmailType.Primary).Email,
                IsEmailConfirmed = false
            };

            return Results.BadRequest("The email address is not confirmed.", response);
        }

        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);
        if (lockoutState.Enabled)
        {
            response = new LoginResponse()
            {
                UserId = user.Id,
                IsLockedOut = lockoutState.Enabled,
                Reason = Mapper.Map(lockoutState.Reason),
            };

            return Results.BadRequest("Account is locked out", response);
        }

        if (!user.HasPassword()) return Results.BadRequest("User doesn't have a password.");

        var isValidPassword = userManager.CheckPassword(user, request.Request.Password);
        if (!isValidPassword)
        {
            user.FailedLoginAttempts += 1;

            var updateResult = await userManager.UpdateAsync(user, cancellationToken);
            if (!updateResult.Succeeded) return updateResult;

            if (user.FailedLoginAttempts < identityOptions.SignIn.MaxFailedLoginAttempts)
            {
                response = new LoginResponse()
                {
                    UserId = user.Id,
                    FailedLoginAttempts = user.FailedLoginAttempts,
                    MaxFailedLoginAttempts = identityOptions.SignIn.MaxFailedLoginAttempts,
                };

                return Results.BadRequest("The password is not valid.", response);
            }

            var deviceBlockResult = await deviceManager.BlockAsync(device, cancellationToken);
            if (!deviceBlockResult.Succeeded) return deviceBlockResult;

            var reason = await reasonManager.FindByTypeAsync(LockoutType.TooManyFailedLoginAttempts, cancellationToken);
            if (reason is null) return Results.NotFound(
                $"Cannot find lockout type {LockoutType.TooManyFailedLoginAttempts}.");

            var lockoutResult = await lockoutManager.BlockAsync(user, reason,
                permanent: true, cancellationToken: cancellationToken);

            if (!lockoutResult.Succeeded) return lockoutResult;

            response = new LoginResponse()
            {
                UserId = user.Id,
                IsLockedOut = true,
                FailedLoginAttempts = user.FailedLoginAttempts,
                MaxFailedLoginAttempts = identityOptions.SignIn.MaxFailedLoginAttempts,
                Reason = Mapper.Map(reason)
            };
        
            return Results.BadRequest("Account is locked out due to too many failed login attempts", response);
        }

        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;

            var userUpdateResult = await userManager.UpdateAsync(user, cancellationToken);
            if (!userUpdateResult.Succeeded) return userUpdateResult;
        }

        if (identityOptions.SignIn.RequireTrustedDevice)
        {
            response = new LoginResponse()
            {
                UserId = user.Id,
                Email = user.Emails.First(x => x.Type is EmailType.Primary).Email,
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

        if (user.HasTwoFactor())
        {
            response = new LoginResponse()
            {
                UserId = user.Id,
                IsTwoFactorEnabled = true,
            };

            return Result.Success(response);
        }
        
        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);

        response = new LoginResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };

        await loginSessionManager.CreateAsync(device, LoginType.Password, cancellationToken: cancellationToken);

        return Result.Success(response, "Successfully logged in.");
    }
}