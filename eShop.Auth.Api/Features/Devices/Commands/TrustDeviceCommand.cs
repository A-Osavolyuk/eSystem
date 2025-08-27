using eShop.Domain.Requests.API.Auth;
using eShop.Domain.Responses.API.Auth;

namespace eShop.Auth.Api.Features.Devices.Commands;

public record TrustDeviceCommand(TrustDeviceRequest Request) : IRequest<Result>;

public class TrustDeviceCommandHandler(
    IUserManager userManager,
    ITokenManager tokenManager,
    IDeviceManager deviceManager,
    ILoginSessionManager loginSessionManager,
    IVerificationManager verificationManager) : IRequestHandler<TrustDeviceCommand, Result>
{
    private readonly IUserManager userManager = userManager;
    private readonly ITokenManager tokenManager = tokenManager;
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly ILoginSessionManager loginSessionManager = loginSessionManager;
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

        if (user.Providers.Any(x => x.Subscribed))
        {
            return Result.Success(new TrustDeviceResponse()
            {
                IsTwoFactorEnabled = true,
                UserId = user.Id
            });
        }
        
        var accessToken = await tokenManager.GenerateAsync(user, TokenType.Access,  cancellationToken);
        var refreshToken = await tokenManager.GenerateAsync(user, TokenType.Refresh,  cancellationToken);

        var sessionResult = await loginSessionManager.CreateAsync(user, device, 
            LoginStatus.Success, LoginType.Password, cancellationToken: cancellationToken);
        
        if (!sessionResult.Succeeded) return sessionResult;
        
        var response = new TrustDeviceResponse()
        {
            UserId = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
        
        return Result.Success(response);
    }
}