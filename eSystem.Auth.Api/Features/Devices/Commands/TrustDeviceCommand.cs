using eSystem.Auth.Api.Security.Session;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Verification;

namespace eSystem.Auth.Api.Features.Devices.Commands;

public record TrustDeviceCommand(TrustDeviceRequest Request) : IRequest<Result>;

public class TrustDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    ISessionManager sessionManager,
    IVerificationManager verificationManager) : IRequestHandler<TrustDeviceCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ISessionManager sessionManager = sessionManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(TrustDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var device = await deviceManager.FindByIdAsync(request.Request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound($"Cannot find user with ID {request.Request.DeviceId}.");
        
        var verificationResult = await verificationManager.VerifyAsync(user, 
            PurposeType.Device, ActionType.Trust, cancellationToken);
        
        if(!verificationResult.Succeeded) return verificationResult;
        
        var deviceResult = await deviceManager.TrustAsync(device, cancellationToken);
        if (!deviceResult.Succeeded) return deviceResult;

        if (user.HasMethods() && user.TwoFactorEnabled)
        {
            return Result.Success(new TrustDeviceResponse()
            {
                IsTwoFactorEnabled = true,
                UserId = user.Id
            });
        }
        
        await sessionManager.CreateAsync(device, cancellationToken);
        
        var response = new TrustDeviceResponse()
        {
            UserId = user.Id,
        };
        
        return Result.Success(response);
    }
}