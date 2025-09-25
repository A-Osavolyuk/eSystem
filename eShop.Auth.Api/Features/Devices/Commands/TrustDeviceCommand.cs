using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Devices.Commands;

public record TrustDeviceCommand(TrustDeviceRequest Request) : IRequest<Result>;

public class TrustDeviceCommandHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    ILoginManager loginManager,
    IAuthorizationManager authorizationManager,
    IVerificationManager verificationManager) : IRequestHandler<TrustDeviceCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ILoginManager loginManager = loginManager;
    private readonly IAuthorizationManager authorizationManager = authorizationManager;
    private readonly IVerificationManager verificationManager = verificationManager;

    public async Task<Result> Handle(TrustDeviceCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.Request.UserId}.");
        
        var device = await deviceManager.FindByIdAsync(request.Request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound($"Cannot find user with ID {request.Request.DeviceId}.");
        
        var verificationResult = await verificationManager.VerifyAsync(user, 
            CodeResource.Device, CodeType.Trust, cancellationToken);
        
        if(!verificationResult.Succeeded) return verificationResult;
        
        var deviceResult = await deviceManager.TrustAsync(device, cancellationToken);
        if (!deviceResult.Succeeded) return deviceResult;

        if (user.HasProviders() && user.TwoFactorEnabled)
        {
            return Result.Success(new TrustDeviceResponse()
            {
                IsTwoFactorEnabled = true,
                UserId = user.Id
            });
        }

        await loginManager.CreateAsync(device, LoginType.Password, cancellationToken);
        await authorizationManager.CreateAsync(device, cancellationToken);
        
        var response = new TrustDeviceResponse()
        {
            UserId = user.Id,
        };
        
        return Result.Success(response);
    }
}