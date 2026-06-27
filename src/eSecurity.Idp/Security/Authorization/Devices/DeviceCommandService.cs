using eSecurity.Idp.Data;
using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;
using eSystem.Core.Primitives.Enums;

namespace eSecurity.Idp.Security.Authorization.Devices;

public sealed class DeviceCommandService(AuthDbContext context, IDeviceQueryService query) : IDeviceCommandService
{
    private readonly AuthDbContext _context = context;
    private readonly IDeviceQueryService _query = query;

    public async ValueTask<Result> CreateAsync(UserDeviceEntity entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _context.UserDevices.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }

    public async ValueTask<Result> BlockAsync(Guid deviceId, CancellationToken cancellationToken = default)
    {
        var device = await _query.GetByIdAsync(deviceId, cancellationToken);
        if (device is null)
        {
            return Results.ClientError(ClientErrorCode.BadRequest, new Error()
            {
                Code = ErrorCode.BadRequest,
                Description = "Invalid device"
            });
        }

        device.IsBlocked = true;
        device.BlockedAt = DateTimeOffset.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        
        return Results.Success(SuccessCodes.Ok);
    }
}