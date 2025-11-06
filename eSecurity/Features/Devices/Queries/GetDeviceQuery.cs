using eSecurity.Common.DTOs;
using eSecurity.Security.Authorization.Devices;

namespace eSecurity.Features.Devices.Queries;

public record GetDeviceQuery(Guid Id) : IRequest<Result>;

public class GetDeviceQueryHandler(IDeviceManager deviceManager) : IRequestHandler<GetDeviceQuery, Result>
{
    private readonly IDeviceManager deviceManager = deviceManager;

    public async Task<Result> Handle(GetDeviceQuery request, CancellationToken cancellationToken)
    {
        var device = await deviceManager.FindByIdAsync(request.Id, cancellationToken);
        if (device is null) return Results.NotFound($"Cannot find device with ID {request.Id}.");
        
        var response = new UserDeviceDto()
        {
            Id = device.Id,
            IpAddress = device.IpAddress,
            OS = device.OS,
            Browser = device.Browser,
            Device = device.Device,
            BlockedDate = device.BlockedDate,
            FirstSeen = device.FirstSeen,
            LastSeen = device.LastSeen,
            UserAgent = device.UserAgent,
            Location = device.Location,
            IsBlocked = device.IsBlocked,
            IsTrusted = device.IsTrusted
        };
        
        return Result.Success(response);
    }
}