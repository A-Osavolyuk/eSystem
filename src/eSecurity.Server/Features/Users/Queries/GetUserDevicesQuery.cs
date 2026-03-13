using eSecurity.Core.Common.DTOs;
using eSecurity.Server.Security.Authorization.Devices;
using eSecurity.Server.Security.Identity.User;
using eSystem.Core.Mediator;
using eSystem.Core.Security.Identity.Claims;

namespace eSecurity.Server.Features.Users.Queries;

public record GetUserDevicesQuery : IRequest<Result>;

public class GetUserDevicesQueryHandler(
    IUserManager userManager,
    IDeviceManager deviceManager,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetUserDevicesQuery, Result>
{
    private readonly IUserManager _userManager = userManager;
    private readonly IDeviceManager _deviceManager = deviceManager;
    private readonly HttpContext _httpContext = httpContextAccessor.HttpContext!;

    public async Task<Result> Handle(GetUserDevicesQuery request, CancellationToken cancellationToken)
    {
        var subjectClaim = _httpContext.User.FindFirst(AppClaimTypes.Sub);
        if (subjectClaim is null) return Results.BadRequest("Invalid subject.");
        
        var user = await _userManager.FindBySubjectAsync(subjectClaim.Value, cancellationToken);
        if (user is null) return Results.NotFound("User not found.");

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
            FirstSeen = device.FirstSeenAt,
            LastSeen = device.LastSeenAt,
            BlockedDate = device.BlockedAt
        }).ToList();
        
        return Results.Ok(response);
    }
}