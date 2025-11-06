using eSecurity.Common.DTOs;
using eSecurity.Security.Authorization.Devices;
using eSecurity.Security.Identity.User;

namespace eSecurity.Features.Users.Queries;

public record GetUserDeviceQuery(Guid UserId, Guid DeviceId) : IRequest<Result>;

public class GetUserDeviceQueryHandler(
    IDeviceManager deviceManager,
    IUserManager userManager) : IRequestHandler<GetUserDeviceQuery, Result>
{
    private readonly IDeviceManager deviceManager = deviceManager;
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserDeviceQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"User with ID {request.UserId} was not found.");

        var device = await deviceManager.FindByIdAsync(request.DeviceId, cancellationToken);
        if (device is null) return Results.NotFound($"Device with ID {request.DeviceId} was not found.");

        var response = new UserDeviceDto()
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
            OS = device.OS,
            UserAgent = device.UserAgent,
        };
        
        return Result.Success(response);
    }
}