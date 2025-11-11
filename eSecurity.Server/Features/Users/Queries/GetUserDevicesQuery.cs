using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Identity.User;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserDevicesQuery(Guid UserId) : IRequest<Result>;

public class GetUserDevicesQueryHandler(IUserManager userManager) : IRequestHandler<GetUserDevicesQuery, Result>
{
    private readonly IUserManager userManager = userManager;

    public async Task<Result> Handle(GetUserDevicesQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId, cancellationToken);
        if (user is null) return Results.NotFound($"Cannot find user with ID {request.UserId}.");

        var response = user.Devices.Select(device => new UserDeviceDto()
        {
            Id = device.Id,
            IsTrusted = device.IsTrusted,
            IsBlocked = device.IsBlocked,
            Browser = device.Browser,
            OS = device.OS,
            IpAddress = device.IpAddress,
            UserAgent = device.UserAgent,
            Device = device.Device,
            Location = device.Location,
            FirstSeen = device.FirstSeen,
            LastSeen = device.LastSeen,
            BlockedDate = device.BlockedDate
        }).ToList();
        
        return Result.Success(response);
    }
}