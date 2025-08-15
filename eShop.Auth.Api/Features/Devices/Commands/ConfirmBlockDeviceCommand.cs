using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Devices.Commands;

public record ConfirmBlockDeviceCommand(ConfirmBlockDeviceRequest Request) : IRequest<Result>;

public class ConfirmBlockDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    ICodeManager codeManager) : IRequestHandler<ConfirmBlockDeviceCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ConfirmBlockDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var device = await deviceManager.FindByIdAsync(request.Request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound($"Cannot find device with ID {request.Request.UserId}.");
        
        var code = request.Request.Code;
        var codeResult = await codeManager.VerifyAsync(user, code, SenderType.Email, 
            CodeType.Block, CodeResource.Device, cancellationToken);

        if (!codeResult.Succeeded) return codeResult;

        var result = await deviceManager.BlockAsync(device, cancellationToken);
        return result;
    }
}