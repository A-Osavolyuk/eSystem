using eSecurity.Core.Common.Requests;
using eSecurity.Server.Security.Authentication.OpenIdConnect.Session;
using eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Constants;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.DeviceCode.Commands;

public sealed record ActivateDeviceCodeCommand(ActivateDeviceCodeRequest Request) : IRequest<Result>;

public sealed class ActivateDeviceCodeCommandHandler(
    IDeviceCodeManager deviceCodeManager,
    IUserManager userManager,
    ISessionManager sessionManager) : IRequestHandler<ActivateDeviceCodeCommand, Result>
{
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;
    private readonly IUserManager _userManager = userManager;
    private readonly ISessionManager _sessionManager = sessionManager;

    public async Task<Result> Handle(ActivateDeviceCodeCommand request, CancellationToken cancellationToken)
    {
        var deviceCode = await _deviceCodeManager.FindByCodeAsync(request.Request.UserCode, cancellationToken);
        if (deviceCode is null) return Results.NotFound("Device code was not found");

        if (deviceCode.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.ExpiredToken,
                Description = "Device code is already expired"
            });
        }

        if (deviceCode.State != DeviceCodeState.Pending)
        {
            return Results.BadRequest(new Error()
            {
                Code = ErrorTypes.OAuth.InvalidToken,
                Description = "Device code is not valid"
            });
        }

        if (request.Request.SessionId.HasValue)
        {
            var session = await _sessionManager.FindByIdAsync(request.Request.SessionId.Value, cancellationToken);
            if (session is null) return Results.NotFound("Session was not found");

            deviceCode.SessionId = session.Id;
        }
        
        var user = await _userManager.FindByIdAsync(request.Request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User was not found");

        deviceCode.UserId = user.Id;
        deviceCode.State = DeviceCodeState.Activated;
        
        return await _deviceCodeManager.UpdateAsync(deviceCode, cancellationToken);
    }
}