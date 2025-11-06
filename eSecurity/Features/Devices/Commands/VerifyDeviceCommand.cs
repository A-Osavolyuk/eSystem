using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Authorization.Devices;
using eSecurity.Security.Identity.User;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Features.Devices.Commands;

public class VerifyDeviceCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}

public class VerifyDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    IVerificationManager verificationManager) : IRequestHandler<VerifyDeviceCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(VerifyDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        
        var device = await deviceManager.FindByIdAsync(request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound($"Cannot find user with ID {request.DeviceId}.");

        var verificationResult = await verificationManager.VerifyAsync(user, 
            PurposeType.Device, ActionType.Verify, cancellationToken);
        
        if(!verificationResult.Succeeded) return verificationResult;
        
        var result = await deviceManager.TrustAsync(device, cancellationToken);
        return result;
    }
}