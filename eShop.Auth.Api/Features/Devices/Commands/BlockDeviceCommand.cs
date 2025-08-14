using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Devices.Commands;

public record BlockDeviceCommand(BlockDeviceRequest Request) : IRequest<Result>;

public class BlockDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager) : IRequestHandler<BlockDeviceCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;

    public async Task<Result> Handle(BlockDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);

        if (user is null)
        {
            return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        }
        
        var device = await deviceManager.FindByIdAsync(request.Request.DeviceId, cancellationToken);
        
        if (device is null)
        {
            return Results.NotFound($"Cannot find device with ID {request.Request.DeviceId}.");
        }
        
        var result = await deviceManager.BlockAsync(device, cancellationToken);
        return result;
    }
}