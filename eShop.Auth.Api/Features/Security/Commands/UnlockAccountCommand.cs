using eShop.Domain.Requests.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record UnlockAccountCommand(UnlockAccountRequest Request) : IRequest<Result>;

public class UnlockAccountCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    ILockoutManager lockoutManager,
    IHttpContextAccessor httpContextAccessor,
    IDeviceManager deviceManager) : IRequestHandler<UnlockAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;
    private readonly IDeviceManager deviceManager = deviceManager;

    public async Task<Result> Handle(UnlockAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = httpContextAccessor.HttpContext?.GetIpV4()!;

        var device = await deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null) return Results.BadRequest("Invalid device.");
        
        var code = request.Request.Code;
        var verificationResult = await codeManager.VerifyAsync(user, code, SenderType.Email, 
            ActionType.Unlock, PurposeType.Account, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        user.FailedLoginAttempts = 0;
        var updateResult = await userManager.UpdateAsync(user, cancellationToken);
        if (!updateResult.Succeeded) return updateResult;
        
        if (device.IsBlocked)
        {
            var deviceUpdateResult = await deviceManager.UnblockAsync(device, cancellationToken);
            if (!deviceUpdateResult.Succeeded) return deviceUpdateResult;
        }

        if (!updateResult.Succeeded) return updateResult;
        var unlockResult = await lockoutManager.UnblockAsync(user, cancellationToken);
        
        return unlockResult;
    }
}