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
    ILoginSessionManager loginSessionManager) : IRequestHandler<TwoFactorLoginCommand, Result>
{
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IUserManager userManager = userManager;
    private readonly ILoginCodeManager loginCodeManager = loginCodeManager;
    private readonly IProviderManager providerManager = providerManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IRecoverManager recoverManager = recoverManager;
    private readonly ILoginSessionManager loginSessionManager = loginSessionManager;

    public async Task<Result> Handle(TwoFactorLoginCommand request,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        var provider = await providerManager.FindByNameAsync(request.Request.Provider, cancellationToken);

        if (provider is null)
        {
            return await NotFoundProviderAsync(user, request.Request.Provider, request.Context, cancellationToken);
        }
        
        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

        if (lockoutState.Enabled)
        {
            return await LockedOutAsync(user, provider, lockoutState, request.Context, cancellationToken);
        }

        var code = request.Request.Code;
        
        var codeResult = await VerifyCodeAsync(user, provider, code, request.Context, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        return await GenerateTokenAsync(user, lockoutState, provider, request.Context, cancellationToken);
    }

    private async Task<Result> NotFoundProviderAsync(UserEntity user, string providerName, HttpContext context,
        CancellationToken cancellationToken)
    {
        var loginSessionResult = await loginSessionManager.CreateAsync(user, context, 
            LoginStatus.Failed, LoginType.TwoFactor, cancellationToken: cancellationToken);
            
        if (!loginSessionResult.Succeeded)
        {
            return loginSessionResult;
        }
            
        return Results.NotFound($"Cannot find provider with name {providerName}.");
    }
    
    private async Task<Result> LockedOutAsync(UserEntity user, ProviderEntity provider, LockoutStateEntity lockoutState,
        HttpContext context, CancellationToken cancellationToken)
    {
        var loginSessionResult = await loginSessionManager.CreateAsync(user, context, 
            LoginStatus.Locked, LoginType.TwoFactor, provider.Name, cancellationToken);
            
        if (!loginSessionResult.Succeeded)
        {
            return loginSessionResult;
        }
            
        return Results.BadRequest($"This user account is locked out with reason: {lockoutState.Reason}.");
    }

    private async Task<Result> VerifyCodeAsync(UserEntity user, ProviderEntity provider, string code, 
        HttpContext context, CancellationToken cancellationToken)
    {
        var codeResult = await loginCodeManager.VerifyAsync(user, provider, code, cancellationToken);

        if (!codeResult.Succeeded)
        {
            var recoveryCodeResult = await recoverManager.VerifyAsync(user, code, cancellationToken);

            if (!recoveryCodeResult.Succeeded)
            {
                var sessionResult = await loginSessionManager.CreateAsync(user, context, 
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
        ProviderEntity provider, HttpContext context, CancellationToken cancellationToken)
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
        
        var result = await loginSessionManager.CreateAsync(user, context, 
            LoginStatus.Success, LoginType.TwoFactor, provider.Name, cancellationToken);
        
        if (!result.Succeeded)
        {
            return result;
        }
        
        return Result.Success(response, "Successfully logged in.");
    }
}