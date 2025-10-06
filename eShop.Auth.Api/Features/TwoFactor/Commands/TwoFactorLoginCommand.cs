using eShop.Domain.Requests.Auth;
using eShop.Domain.Responses.Auth;

namespace eShop.Auth.Api.Features.TwoFactor.Commands;

public sealed record TwoFactorLoginCommand(TwoFactorLoginRequest Request)
    : IRequest<Result>;

public sealed class LoginWith2FaCommandHandler(
    IUserManager userManager,
    ITwoFactorManager twoFactorManager,
    ILockoutManager lockoutManager,
    IRecoverManager recoverManager,
    ILoginManager loginManager,
    IAuthorizationManager authorizationManager,
    IDeviceManager deviceManager,
    IReasonManager reasonManager,
    ICodeManager codeManager,
    IHttpContextAccessor httpContextAccessor,
    IdentityOptions identityOptions) : IRequestHandler<TwoFactorLoginCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITwoFactorManager twoFactorManager = twoFactorManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IRecoverManager recoverManager = recoverManager;
    private readonly ILoginManager loginManager = loginManager;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IReasonManager reasonManager = reasonManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(TwoFactorLoginCommand request,
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
        
        var provider = await twoFactorManager.FindByTypeAsync(request.Request.Type, cancellationToken);
        if (provider is null) 
            return Results.NotFound($"Cannot find provider with type {request.Request.Type}.");
        
        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);
        if (lockoutState.Enabled) 
            return Results.BadRequest($"This user account is locked out with reason: {lockoutState.Reason}.");

        var code = request.Request.Code;

        var sender = provider.Type switch
        {
            MethodType.Sms => SenderType.Sms,
            MethodType.AuthenticatorApp => SenderType.AuthenticatorApp,
            _ => throw new NotSupportedException("Unknown provider type")
        };
        
        var codeResult = await codeManager.VerifyAsync(user, code, sender, 
            CodeType.SignIn, CodeResource.TwoFactor, cancellationToken);
        
        if (!codeResult.Succeeded)
        {
            var recoveryCodeResult = await recoverManager.VerifyAsync(user, code, cancellationToken);
            if (!recoveryCodeResult.Succeeded)
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

                var reason = await reasonManager.FindByTypeAsync(
                    LockoutType.TooManyFailedLoginAttempts, cancellationToken);
                
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
        }

        if (user.FailedLoginAttempts > 0)
        {
            user.FailedLoginAttempts = 0;
            
            var userUpdateResult = await userManager.UpdateAsync(user, cancellationToken);
            if (!userUpdateResult.Succeeded) return userUpdateResult;
        }
        
        response = new LoginResponse() { UserId = user.Id, };
        
        await loginManager.CreateAsync(device, LoginType.TwoFactor, provider.Type.ToString(), cancellationToken);
        await authorizationManager.CreateAsync(device, cancellationToken);
        
        return Result.Success(response);
    }
}