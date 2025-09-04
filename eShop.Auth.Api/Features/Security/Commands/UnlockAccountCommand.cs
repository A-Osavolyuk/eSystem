using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record UnlockAccountCommand(UnlockAccountRequest Request, HttpContext Context) : IRequest<Result>;

public class RecoverAccountCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    ILockoutManager lockoutManager,
    IDeviceManager deviceManager) : IRequestHandler<UnlockAccountCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly ILockoutManager lockoutManager = lockoutManager;
    private readonly IDeviceManager deviceManager = deviceManager;

    public async Task<Result> Handle(UnlockAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var userAgent = RequestUtils.GetUserAgent(request.Context);
        var ipAddress = RequestUtils.GetIpV4(request.Context);

        var device = await deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null) return Results.BadRequest("Invalid device.");
        
        var code = request.Request.Code;
        var verificationResult = await codeManager.VerifyAsync(user, code, SenderType.Email, 
            CodeType.Unlock, CodeResource.Account, cancellationToken);

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