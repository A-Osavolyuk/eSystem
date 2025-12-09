using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserDevicesQuery(Guid UserId) : IRequest<Result>;

public class GetUserDevicesQueryHandler(
    IUserManager userManager,
    IDeviceManager deviceManager) : IRequestHandler<GetUserDevicesQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;

    public async Task<Result> Handle(GetUserDevicesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var devices = await _deviceManager.GetAllAsync(user, cancellationToken);
        var response = devices.Select(device => new UserDeviceDto()
        {
            Id = device.Id,
            IsTrusted = device.IsTrusted,
            IsBlocked = device.IsBlocked,
            Browser = device.Browser,
            Os = device.Os,
            IpAddress = device.IpAddress,
            UserAgent = device.UserAgent,
            Device = device.Device,
            Location = device.Location,
            FirstSeen = device.FirstSeen,
            LastSeen = device.LastSeen,
            BlockedDate = device.BlockedDate
        }).ToList();
        
        return Results.Ok(response);
    }
}