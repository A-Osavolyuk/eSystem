using System.Text.Json.Serialization;
using eSecurity.Core.Responses;
using eSecurity.Idp.Security.Authorization.Token.DeviceCode;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.DeviceCode;

public sealed class CheckDeviceCodeCommand : IRequest<Result>
{
    [JsonPropertyName("user_code")]
    public required string UserCode { get; set; }
}

public sealed class CheckDeviceCodeCommandHandler(
    IDeviceCodeManager deviceCodeManager) : IRequestHandler<CheckDeviceCodeCommand, Result>
{
    private readonly IDeviceCodeManager _deviceCodeManager = deviceCodeManager;

    public async Task<Result> Handle(CheckDeviceCodeCommand request, CancellationToken cancellationToken)
    {
        var deviceCode = await _deviceCodeManager.FindByCodeAsync(request.UserCode, cancellationToken);
        if (deviceCode is null)
        {
            return Results.Success(SuccessCodes.Ok, new CheckDeviceCodeResponse
            {
                Exists = false
            });
        }

        if (deviceCode.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return Results.Success(SuccessCodes.Ok, new CheckDeviceCodeResponse
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
        
        return Results.Success(SuccessCodes.Ok, response);
    }
}

public sealed class CheckDeviceCodeCommandValidator : IRequestValidator<CheckDeviceCodeCommand>
{
    public async ValueTask<Result> Validate(CheckDeviceCodeCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.UserCode))
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.InvalidRequest,
                Description = "'user_code' is required"
            });
        }
        
        return Results.Success(SuccessCodes.Ok);
    }
}