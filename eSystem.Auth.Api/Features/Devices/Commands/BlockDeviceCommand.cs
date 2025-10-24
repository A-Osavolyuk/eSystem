using eSystem.Domain.Requests.Auth;
using eSystem.Domain.Security.Verification;

namespace eSystem.Auth.Api.Features.Devices.Commands;

public record BlockDeviceCommand(BlockDeviceRequest Request) : IRequest<Result>;

public class BlockDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    IVerificationManager verificationManager) : IRequestHandler<BlockDeviceCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(BlockDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var device = await deviceManager.FindByIdAsync(request.Request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound($"Cannot find device with ID {request.Request.DeviceId}.");

        var verificationResult = await verificationManager.VerifyAsync(user,
            PurposeType.Device, ActionType.Block, cancellationToken);

        if (!verificationResult.Succeeded) return verificationResult;

        var result = await deviceManager.BlockAsync(device, cancellationToken);
        return result;
    }
}