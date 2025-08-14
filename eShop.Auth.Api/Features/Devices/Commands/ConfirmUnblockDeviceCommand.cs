using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Devices.Commands;

public record ConfirmUnblockDeviceCommand(ConfirmUnblockDeviceRequest Request) : IRequest<Result>;

public class ConfirmUnblockDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    ICodeManager codeManager) : IRequestHandler<ConfirmVerifyDeviceCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ICodeManager codeManager = codeManager;

    public async Task<Result> Handle(ConfirmVerifyDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");

        var device = await deviceManager.FindByIdAsync(request.Request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound($"Cannot find device with ID {request.Request.UserId}.");
        
        var code = request.Request.Code;
        var codeResult = await codeManager.VerifyAsync(user, code, SenderType.Email, 
            CodeType.Unblock, CodeResource.Device, cancellationToken);

        if (!codeResult.Succeeded) return codeResult;

        var result = await deviceManager.UnblockAsync(device, cancellationToken);
        return result;
    }
}