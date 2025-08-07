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
            await loginSessionManager.CreateAsync(user, request.Context, 
                LoginStatus.Failed, LoginType.TwoFactor, cancellationToken: cancellationToken);
            
            return Results.NotFound($"Cannot find provider with name {request.Request.Provider}.");
        }
        
        var lockoutState = await lockoutManager.FindAsync(user, cancellationToken);

        if (lockoutState.Enabled)
        {
            await loginSessionManager.CreateAsync(user, request.Context, 
                LoginStatus.Locked, LoginType.TwoFactor, provider.Name, cancellationToken);
            
            return Results.BadRequest($"This user account is locked out with reason: {lockoutState.Reason}.");
        }

        var code = request.Request.Code;
        
        var result = await loginCodeManager.VerifyAsync(user, provider, code, cancellationToken);

        if (!result.Succeeded)
        {
            var recoveryCodeResult = await recoverManager.VerifyAsync(user, code, cancellationToken);

            if (!recoveryCodeResult.Succeeded)
            {
                await loginSessionManager.CreateAsync(user, request.Context, 
                    LoginStatus.Failed, LoginType.TwoFactor, provider.Name, cancellationToken);
                
                return Results.BadRequest($"Invalid two-factor code {request.Request.Code}.");
            }
        }

        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access, cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh, cancellationToken);

        var response = new LoginResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            IsLockedOut = lockoutState.Enabled,
        };
        
        await loginSessionManager.CreateAsync(user, request.Context, 
            LoginStatus.Success, LoginType.TwoFactor, provider.Name, cancellationToken);
        
        return Result.Success(response, "Successfully logged in.");
    }
}