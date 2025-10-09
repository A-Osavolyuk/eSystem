using eShop.Auth.Api.Security.Protection;
using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public sealed record AuthenticatorSignInCommand(AuthenticatorSignInRequest Request)
    : IRequest<Result>;

public sealed class LoginWith2FaCommandHandler(
    IUserManager userManager,
    ILockoutManager lockoutManager,
    ILoginManager loginManager,
    IAuthorizationManager authorizationManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor,
    ISecretManager secretManager,
    SecretProtector protector,
    IdentityOptions identityOptions) : IRequestHandler<AuthenticatorSignInCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly ILoginManager loginManager = loginManager;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly ISecretManager secretManager = secretManager;
    private readonly SecretProtector protector = protector;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(AuthenticatorSignInCommand request,
        CancellationToken cancellationToken)
    {
        LoginResponse? response;

        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = httpContextAccessor.HttpContext?.GetIpV4()!;
        var clientInfo = httpContextAccessor.HttpContext?.GetClientInfo()!;

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
        
        if (user.LockoutState.Enabled)
            return Results.BadRequest($"This user account is locked out with reason: {user.LockoutState.Type}.");

        var code = request.Request.Code;
        var userSecret = await secretManager.FindAsync(user, cancellationToken);
        if (userSecret is null) return Results.NotFound("Not found user secret");

        var unprotectedSecret = protector.Unprotect(userSecret.Secret);
        var isVerifiedCode = AuthenticatorUtils.VerifyCode(code, unprotectedSecret);

        if (!isVerifiedCode)
        {
            user.FailedLoginAttempts += 1;

            if (user.FailedLoginAttempts < identityOptions.SignIn.MaxFailedLoginAttempts)
            {
                response = new LoginResponse()
                {
                    UserId = user.Id,
                    FailedLoginAttempts = user.FailedLoginAttempts,
                    MaxFailedLoginAttempts = identityOptions.SignIn.MaxFailedLoginAttempts,
                };

                return Results.BadRequest($"Invalid two-factor code {code}.", response);
            }

            var deviceBlockResult = await deviceManager.BlockAsync(device, cancellationToken);
            if (!deviceBlockResult.Succeeded) return deviceBlockResult;

            var lockoutResult = await lockoutManager.BlockAsync(user, LockoutType.TooManyFailedLoginAttempts,
                permanent: true, cancellationToken: cancellationToken);

            if (!lockoutResult.Succeeded) return lockoutResult;

            response = new LoginResponse()
            {
                UserId = user.Id,
                IsLockedOut = true,
                FailedLoginAttempts = user.FailedLoginAttempts,
                MaxFailedLoginAttempts = identityOptions.SignIn.MaxFailedLoginAttempts,
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

        response = new LoginResponse() { UserId = user.Id, };

        const string method = nameof(TwoFactorMethod.AuthenticatorApp);
        await loginManager.CreateAsync(device, LoginType.TwoFactor, method, cancellationToken);
        await authorizationManager.CreateAsync(device, cancellationToken);

        return Result.Success(response);
    }
}