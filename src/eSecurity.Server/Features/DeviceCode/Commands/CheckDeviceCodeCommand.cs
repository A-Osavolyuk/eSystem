using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;
using eSystem.Core.Mediator;

namespace eSecurity.Server.Features.DeviceCode.Commands;

public record CheckDeviceCodeCommand(CheckDeviceCodeRequest Request) : IRequest<Result>;

public class CheckDeviceCodeCommandHandler(
    IDeviceCodeManager deviceCodeManager) : IRequestHandler<CheckDeviceCodeCommand, Result>
{
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;

    public async Task<Result> Handle(CheckDeviceCodeCommand request, CancellationToken cancellationToken)
    {
        var deviceCode = await _deviceCodeManager.FindByCodeAsync(request.Request.UserCode, cancellationToken);
        if (deviceCode is null)
        {
            return Results.Ok(new CheckDeviceCodeResponse()
            {
                Exists = false
            });
        }

        if (deviceCode.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return Results.Ok(new CheckDeviceCodeResponse()
            {
                Exists = true,
                IsExpired = true
            });
        }

        var response = deviceCode.State switch
        {
            DeviceCodeState.Approved => new CheckDeviceCodeResponse { Exists = true, IsActivated = true },
            DeviceCodeState.Denied => new CheckDeviceCodeResponse { Exists = true, IsDenied = true },
            DeviceCodeState.Consumed => new CheckDeviceCodeResponse { Exists = true, IsConsumed = true },
            DeviceCodeState.Pending => new CheckDeviceCodeResponse { Exists = true },
            _ => throw new NotSupportedException("Unsupported device code state")
        };
        
        return Results.Ok(response);
    }
}