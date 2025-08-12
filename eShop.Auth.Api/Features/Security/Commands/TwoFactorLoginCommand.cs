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
    IDeviceManager deviceManager) : IRequestHandler<TwoFactorLoginCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly ILoginCodeManager loginCodeManager = loginCodeManager;
    private readonly IProviderManager providerManager = providerManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IRecoverManager recoverManager = recoverManager;
    private readonly ILoginSessionManager loginSessionManager = loginSessionManager;
    private readonly IDeviceManager deviceManager = deviceManager;

    public async Task<Result> Handle(TwoFactorLoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
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
        {
            return await NotFoundProviderAsync(user, request.Request.Provider, device, cancellationToken);
        }
        
        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

        if (lockoutState.Enabled)
        {
            return await LockedOutAsync(user, provider, lockoutState, device, cancellationToken);
        }

        var code = request.Request.Code;
        
        var codeResult = await VerifyCodeAsync(user, provider, code, device, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        return await GenerateTokenAsync(user, lockoutState, provider, device, cancellationToken);
    }

    private async Task<Result> NotFoundProviderAsync(UserEntity user, string providerName, UserDeviceEntity device,
        CancellationToken cancellationToken)
    {
        var loginSessionResult = await loginSessionManager.CreateAsync(user, device, 
            LoginStatus.Failed, LoginType.TwoFactor, cancellationToken: cancellationToken);
            
        if (!loginSessionResult.Succeeded)
        {
            return loginSessionResult;
        }
            
        return Results.NotFound($"Cannot find provider with name {providerName}.");
    }
    
    private async Task<Result> LockedOutAsync(UserEntity user, ProviderEntity provider, LockoutStateEntity lockoutState,
        UserDeviceEntity device, CancellationToken cancellationToken)
    {
        var loginSessionResult = await loginSessionManager.CreateAsync(user, device, 
            LoginStatus.Locked, LoginType.TwoFactor, provider.Name, cancellationToken);
            
        if (!loginSessionResult.Succeeded)
        {
            return loginSessionResult;
        }
            
        return Results.BadRequest($"This user account is locked out with reason: {lockoutState.Reason}.");
    }

    private async Task<Result> VerifyCodeAsync(UserEntity user, ProviderEntity provider, string code, 
        UserDeviceEntity device, CancellationToken cancellationToken)
    {
        var codeResult = await loginCodeManager.VerifyAsync(user, provider, code, cancellationToken);

        if (!codeResult.Succeeded)
        {
            var recoveryCodeResult = await recoverManager.VerifyAsync(user, code, cancellationToken);

            if (!recoveryCodeResult.Succeeded)
            {
                var sessionResult = await loginSessionManager.CreateAsync(user, device, 
                    LoginStatus.Failed, LoginType.TwoFactor, provider.Name, cancellationToken);
                
                if (!sessionResult.Succeeded)
                {
                    return sessionResult;
                }
                
                return Results.BadRequest($"Invalid two-factor code {code}.");
            }
        }

        return Result.Success();
    }

    private async Task<Result> GenerateTokenAsync(UserEntity user, LockoutStateEntity lockoutState, 
        ProviderEntity provider, UserDeviceEntity device, CancellationToken cancellationToken)
    {
        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);

        var response = new LoginResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            IsLockedOut = lockoutState.Enabled,
        };
        
        var result = await loginSessionManager.CreateAsync(user, device, 
            LoginStatus.Success, LoginType.TwoFactor, provider.Name, cancellationToken);
        
        if (!result.Succeeded)
        {
            return result;
        }
        
        return Result.Success(response, "Successfully logged in.");
    }
}