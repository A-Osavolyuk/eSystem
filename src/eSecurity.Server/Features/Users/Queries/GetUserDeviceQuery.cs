using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserDeviceQuery(Guid UserId, Guid DeviceId) : IRequest<Result>;

public class GetUserDeviceQueryHandler(
    IDeviceManager deviceManager,
    IUserManager userManager) : IRequestHandler<GetUserDeviceQuery, Result>
{
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly IUserManager _userManager = userManager;

    public async Task<Result> Handle(GetUserDeviceQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

        var device = await _deviceManager.FindByIdAsync(request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound("Device not found.");

        var response = new UserDeviceDto
        {
            Id = device.Id,
            Browser = device.Browser,
            Device = device.Device,
            FirstSeen = device.FirstSeen,
            BlockedDate = device.BlockedDate,
            IpAddress = device.IpAddress,
            IsBlocked = device.IsBlocked,
            IsTrusted = device.IsTrusted,
            LastSeen = device.LastSeen,
            Location = device.Location,
            Os = device.Os,
            UserAgent = device.UserAgent,
        };
        
        return Results.Ok(response);
    }
}