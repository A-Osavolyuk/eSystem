using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Authorization.Devices;
using eSecurity.Security.Identity.User;

namespace eSecurity.Features.Devices.Commands;

public class UnblockDeviceCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}

public class UnblockDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    IVerificationManager verificationManager) : IRequestHandler<UnblockDeviceCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(UnblockDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        
        var device = await deviceManager.FindByIdAsync(request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound($"Cannot find device with ID {request.DeviceId}.");
        
        var verificationResult = await verificationManager.VerifyAsync(user, 
            PurposeType.Device, ActionType.Unblock, cancellationToken);
        
        if(!verificationResult.Succeeded) return verificationResult;
        
        var result = await deviceManager.UnblockAsync(device, cancellationToken);
        return result;
    }
}