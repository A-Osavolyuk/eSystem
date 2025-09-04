using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public sealed record TwoFactorLoginCommand(TwoFactorLoginRequest Request, HttpContext Context)
    : IRequest<Result>;

public sealed class LoginWith2FaCommandHandler(
    ITokenManager tokenManager,
    IUserManager userManager,
    ILoginCodeManager loginCodeManager,
    IProviderManager providerManager,
    ILockoutManager lockoutManager,
    IRecoverManager recoverManager,
    ILoginSessionManager loginSessionManager,
    IDeviceManager deviceManager,
    IReasonManager reasonManager,
    IdentityOptions identityOptions) : IRequestHandler<TwoFactorLoginCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly ILoginCodeManager loginCodeManager = loginCodeManager;
    private readonly IProviderManager providerManager = providerManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IRecoverManager recoverManager = recoverManager;
    private readonly ILoginSessionManager loginSessionManager = loginSessionManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IReasonManager reasonManager = reasonManager;
    private readonly IdentityOptions identityOptions = identityOptions;

    public async Task<Result> Handle(TwoFactorLoginCommand request,
        CancellationToken cancellationToken)
    {
        LoginResponse? response;
        
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
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
        
        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);
        if (provider is null) 
            return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        
        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);
        if (lockoutState.Enabled) 
            return Results.BadRequest($"This user account is locked out with reason: {lockoutState.Reason}.");

        var code = request.Request.Code;
        
        var codeResult = await loginCodeManager.VerifyAsync(user, provider, code, cancellationToken);
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
        
        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);

        response = new LoginResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
        
        await loginSessionManager.CreateAsync(device, LoginType.TwoFactor, provider.Name, cancellationToken);
        return Result.Success(response, "Successfully logged in.");
    }
}