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
        
        var response = Mapper.Map(device);
        return Result.Success(response);
    }
}