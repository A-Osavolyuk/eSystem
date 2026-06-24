using eSecurity.Core.DTOs;
using eSecurity.Idp.Security.Authorization.Devices;
using eSecurity.Idp.Security.Identity.User;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Features.Users;

public record GetUserDevicesQuery : IRequest<Result>;

public class GetUserDevicesQueryHandler(
    ICurrentUserAccessor currentUserAccessor,
    IDeviceManager deviceManager) : IRequestHandler<GetUserDevicesQuery, Result>
{
    private readonly ICurrentUserAccessor _currentUserAccessor = currentUserAccessor;
    private readonly IDeviceManager _deviceManager = deviceManager;

    public async Task<Result> Handle(GetUserDevicesQuery request, CancellationToken cancellationToken)
    {
        var user = await _currentUserAccessor.GetRequiredCurrentAsync(cancellationToken);
        var devices = await _deviceManager.GetAllAsync(user, cancellationToken);
        var response = devices.Select(device => new UserDeviceDto
        {
            Id = device.Id,
            IsBlocked = device.IsBlocked,
            Browser = device.Browser,
            Os = device.Os,
            IpAddress = device.IpAddress,
            UserAgent = device.UserAgent,
            Device = device.Device,
            Location = device.Location,
            FirstSeenAt = device.FirstSeenAt,
            LastSeenAt = device.LastSeenAt,
            BlockedAt = device.BlockedAt
        }).ToList();
        
        return Results.Success(SuccessCodes.Ok, response);
    }
}