using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record UnblockDeviceCommand(UnblockDeviceRequest Request) : IRequest<Result>;

public class UnblockDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager) : IRequestHandler<UnblockDeviceCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;

    public async Task<Result> Handle(UnblockDeviceCommand request, CancellationToken cancellationToken)
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
        
        var result = await deviceManager.UnblockAsync(device, cancellationToken);
        return result;
    }
}