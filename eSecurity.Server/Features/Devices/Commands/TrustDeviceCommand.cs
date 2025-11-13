using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authorization.Access;
using eSecurity.Server.Security.Authentication.Oidc.Session;
using eSecurity.Server.Security.Authorization.Access.Verification;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Devices.Commands;

public record TrustDeviceCommand(TrustDeviceRequest Request) : IRequest<Result>;

public class TrustDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    ISessionManager sessionManager,
    IVerificationManager verificationManager) : IRequestHandler<TrustDeviceCommand, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IVerificationManager _verificationManager = verificationManager;

    public async Task<Result> Handle(TrustDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var device = await _deviceManager.FindByIdAsync(request.Request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound($"Cannot find user with ID {request.Request.DeviceId}.");
        
        var verificationResult = await _verificationManager.VerifyAsync(user, 
            PurposeType.Device, ActionType.Trust, cancellationToken);
        
        if(!verificationResult.Succeeded) return verificationResult;
        
        var deviceResult = await _deviceManager.TrustAsync(device, cancellationToken);
        if (!deviceResult.Succeeded) return deviceResult;

        if (user.HasMethods() && user.TwoFactorEnabled)
        {
            return Result.Success(new TrustDeviceResponse()
            {
                IsTwoFactorEnabled = true,
                UserId = user.Id
            });
        }
        
        await _sessionManager.CreateAsync(device, cancellationToken);
        
        var response = new TrustDeviceResponse()
        {
            UserId = user.Id,
        };
        
        return Result.Success(response);
    }
}