using eShop.Auth.Api.Security.Identity;
using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public record VerifyRecoveryCodeCommand(VerifyRecoveryCodeRequest Request) : IRequest<Result>;

public class VerifyRecoveryCodeCommandHandler(
    IUserManager userManager,
    IRecoverManager recoveryManager,
    IHttpContextAccessor httpContextAccessor,
    IDeviceManager deviceManager,
    ILoginManager loginManager,
    IAuthorizationManager authorizationManager,
    ILockoutManager lockoutManager,
    IOptions<SignInOptions> options) : IRequestHandler<VerifyRecoveryCodeCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IRecoverManager recoveryManager = recoveryManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ILoginManager loginManager = loginManager;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly HttpContext httpContext = httpContextAccessor.HttpContext!;
    private readonly SignInOptions options = options.Value;

    public async Task<Result> Handle(VerifyRecoveryCodeCommand request, CancellationToken cancellationToken)
    {
        VerifyRecoveryCodeResponse response;
        
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}");
        
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

        var codeResult = await recoveryManager.VerifyAsync(user, request.Request.Code, cancellationToken);
        if (!codeResult.Succeeded)
        {
            user.FailedLoginAttempts += 1;

            if (user.FailedLoginAttempts < options.MaxFailedLoginAttempts)
            {
                response = new VerifyRecoveryCodeResponse()
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

            response = new VerifyRecoveryCodeResponse()
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

        response = new VerifyRecoveryCodeResponse() { UserId = user.Id, };

        const string method = nameof(TwoFactorMethod.AuthenticatorApp);
        await loginManager.CreateAsync(device, LoginType.TwoFactor, method, cancellationToken);
        await authorizationManager.CreateAsync(device, cancellationToken);

        return Result.Success(response);
    }
}