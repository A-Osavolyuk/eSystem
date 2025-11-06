using eSecurity.Security.Authentication.Odic.Session;
using eSecurity.Security.Authorization.Access;
using eSecurity.Security.Authorization.Devices;
using eSecurity.Security.Identity.User;
using eSystem.Core.Requests.Auth;
using eSystem.Core.Responses.Auth;
using eSystem.Core.Security.Authorization.Access;

namespace eSecurity.Features.Devices.Commands;

public class TrustDeviceCommand() : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid DeviceId { get; set; }
}

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
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");
        
        var device = await deviceManager.FindByIdAsync(request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound($"Cannot find user with ID {request.DeviceId}.");
        
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