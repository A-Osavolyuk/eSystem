using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Security.Commands;

public record VerifyDeviceCommand(VerifyDeviceRequest Request) : IRequest<Result>;

public class VerifyDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager) : IRequestHandler<VerifyDeviceCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;

    public async Task<Result> Handle(VerifyDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var device = await deviceManager.FindByIdAsync(request.Request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound($"Cannot find user with ID {request.Request.DeviceId}.");
        
        var deviceResult = await deviceManager.TrustAsync(device, cancellationToken);
        return deviceResult;
    }
}