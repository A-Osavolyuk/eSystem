using eSecurity.Core.Common.Requests;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Devices.Commands;

public record VerifyDeviceCommand(VerifyDeviceRequest Request) : IRequest<Result>;

public class VerifyDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyDeviceCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");
        
        var device = await _deviceManager.FindByIdAsync(request.Request.DeviceId, cancellationToken);
        if (device is null || device.IsBlocked || !device.IsTrusted)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.Common.InvalidDevice,
                Description = "Invalid device."
            });
        }

        var verificationResult = await _verificationManager.VerifyAsync(user, 
            PurposeType.Device, ActionType.Verify, cancellationToken);
        
        if(!verificationResult.Succeeded) return verificationResult;
        
        var result = await _deviceManager.TrustAsync(device, cancellationToken);
        return result;
    }
}