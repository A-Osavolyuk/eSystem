using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.Devices.Commands;

public record UnblockDeviceCommand(UnblockDeviceRequest Request) : IRequest<Result>;

public class UnblockDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    IVerificationManager verificationManager) : IRequestHandler<UnblockDeviceCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(UnblockDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found");
        
        var device = await _deviceManager.FindByIdAsync(request.Request.DeviceId, cancellationToken);
        if (device is null || device.IsBlocked)
        {
            return Results.BadRequest(new Error
            {
                Code = ErrorTypes.Common.InvalidDevice,
                Description = "Invalid device."
            });
        }
        
        var verificationResult = await _verificationManager.VerifyAsync(user, 
            PurposeType.Device, ActionType.Unblock, cancellationToken);
        
        if(!verificationResult.Succeeded) return verificationResult;
        
        var result = await _deviceManager.UnblockAsync(device, cancellationToken);
        return result;
    }
}