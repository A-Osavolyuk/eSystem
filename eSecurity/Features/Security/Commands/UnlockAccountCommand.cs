using eSecurity.Security.Authentication.Lockout;
using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Authorization.Access.Codes;
using eSecurity.Security.Authorization.Devices;
using eSecurity.Security.Identity.User;
using eSystem.Core.Common.Http.Context;
using eSystem.Core.Common.Messaging;

namespace eSecurity.Features.Security.Commands;

public record UnlockAccountCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}

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
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var userAgent = httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = httpContextAccessor.HttpContext?.GetIpV4()!;
        var device = user.GetDevice(userAgent, ipAddress);
        if (device is null) return Results.BadRequest("Invalid device.");
        
        var code = request.Code;
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