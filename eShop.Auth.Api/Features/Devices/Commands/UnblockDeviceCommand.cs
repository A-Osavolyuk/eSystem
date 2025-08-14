using eShop.Auth.Api.Messages.Email;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Auth.Api.Features.Devices.Commands;

public record UnblockDeviceCommand(UnblockDeviceRequest Request) : IRequest<Result>;

public class UnblockDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    ICodeManager codeManager,
    IMessageService messageService) : IRequestHandler<UnblockDeviceCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ICodeManager codeManager = codeManager;
    private readonly IMessageService messageService = messageService;

    public async Task<Result> Handle(UnblockDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var device = await deviceManager.FindByIdAsync(request.Request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound($"Cannot find device with ID {request.Request.DeviceId}.");
        
        var code = await codeManager.GenerateAsync(user!, SenderType.Email, CodeType.Unblock, 
            CodeResource.Device, cancellationToken);
        
        var message = new UnblockDeviceMessage()
        {
            Credentials = new ()
            {
                { "To", user!.Email },
                { "Subject", "Unblock block" }
            },
            Payload = new()
            {
                { "Code", code },
                { "UserName", user.UserName },
                { "Ip", device.IpAddress! },
                { "OS", device.OS! },
                { "Device", device.Device! },
                { "Browser", device.Browser! }
            }
        };

        await messageService.SendMessageAsync(SenderType.Email, message, cancellationToken);

        return Result.Success();
    }
}