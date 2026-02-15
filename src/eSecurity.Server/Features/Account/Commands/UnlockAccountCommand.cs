using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authentication.Lockout;
using eSecurity.Server.Security.Authorization.Access.Codes;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Cryptography.Hashing;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Extensions;
using eSystem.Core.Common.Messaging;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Account.Commands;

public record UnlockAccountCommand(UnlockAccountRequest Request) : IRequest<Result>;

public class UnlockAccountCommandHandler(
    IUserManager userManager,
    ICodeManager codeManager,
    ILockoutManager lockoutManager,
    IHttpContextAccessor httpContextAccessor,
    IHasherProvider hasherProvider,
    IDeviceManager deviceManager) : IRequestHandler<UnlockAccountCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly ICodeManager _codeManager = codeManager;
    private readonly ILockoutManager _lockoutManager = lockoutManager;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IHasher _hasher = hasherProvider.GetHasher(HashAlgorithm.Pbkdf2);

    public async Task<Result> Handle(UnlockAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found");

        var userAgent = _httpContextAccessor.HttpContext?.GetUserAgent()!;
        var ipAddress = _httpContextAccessor.HttpContext?.GetIpV4()!;
        var device = await _deviceManager.FindAsync(user, userAgent, ipAddress, cancellationToken);
        if (device is null || device.IsBlocked)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidDevice,
                Description = "Invalid device"
            });

        }
        
        var codeHash = _hasher.Hash(request.Request.Code);
        var code = await _codeManager.FindAsync(user, codeHash, cancellationToken);
        if (code is null) return Results.NotFound("Code not found");

        var codeResult = await _codeManager.RemoveAsync(code, cancellationToken);
        if (!codeResult.Succeeded) return codeResult;

        user.FailedLoginAttempts = 0;
        var updateResult = await _userManager.UpdateAsync(user, cancellationToken);
        if (!updateResult.Succeeded) return updateResult;
        
        if (device.IsBlocked)
        {
            var deviceUpdateResult = await _deviceManager.UnblockAsync(device, cancellationToken);
            if (!deviceUpdateResult.Succeeded) return deviceUpdateResult;
        }

        if (!updateResult.Succeeded) return updateResult;
        var unlockResult = await _lockoutManager.UnblockAsync(user, cancellationToken);
        
        return unlockResult;
    }
}