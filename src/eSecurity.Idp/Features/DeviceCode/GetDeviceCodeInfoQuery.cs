using eSecurity.Core.DTOs;
using eSecurity.Idp.Security.Authentication.Client;
using eSecurity.Idp.Security.Authorization.Token.DeviceCode;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.DeviceCode;

public sealed record GetDeviceCodeInfoQuery(string UserCode) : IRequest<Result>;

public sealed class GetDeviceCodeInfoQueryHandler(
    IDeviceCodeManager deviceCodeManager,
    IClientManager clientManager) : IRequestHandler<GetDeviceCodeInfoQuery, Result>
{
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;
    private readonly IClientManager _clientManager = clientManager;

    public async Task<Result> Handle(GetDeviceCodeInfoQuery request, CancellationToken cancellationToken)
    {
        var deviceCode = await _deviceCodeManager.FindByCodeAsync(request.UserCode, cancellationToken);
        if (deviceCode is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Device code not found"
            });
        }

        if (deviceCode.State is not DeviceCodeState.Pending)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error
            {
                Code = ErrorCode.InvalidToken,
                Description = "This device code is not available anymore"
            });
        }

        var client = await _clientManager.FindByIdAsync(deviceCode.ClientId, cancellationToken);
        if (client is null)
        {
            return Results.ClientError(ClientErrorCode.NotFound, new Error
            {
                Code = ErrorCode.NotFound,
                Description = "Client not found"
            });
        }

        var response = new DeviceCodeInfo
        {
            ClientName = client.Name,
            DeviceModel = deviceCode.DeviceModel,
            DeviceName = deviceCode.DeviceName,
            Scopes = deviceCode.Scopes.Select(x => x.Scope).ToArray()
        };
        
        return Results.Success(SuccessCodes.Ok, response);
    }
}