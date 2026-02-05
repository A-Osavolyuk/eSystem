using eSecurity.Server.Data;
using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.DeviceCode;

public sealed class DeviceCodeManager(AuthDbContext context) : IDeviceCodeManager
{
    private readonly AuthDbContext _context = context;

    public async ValueTask<DeviceCodeEntity?> FindByHashAsync(string deviceCodeHash,
        CancellationToken cancellationToken = default)
    {
        return await _context.DeviceCodes.FirstOrDefaultAsync(
            x => x.DeviceCodeHash == deviceCodeHash, cancellationToken);
    }

    public async ValueTask<DeviceCodeEntity?> FindByCodeAsync(string userCode,
        CancellationToken cancellationToken = default)
    {
        return await _context.DeviceCodes.FirstOrDefaultAsync(
            x => x.UserCode == userCode, cancellationToken);
    }

    public async ValueTask<Result> CreateAsync(DeviceCodeEntity deviceCode,
        CancellationToken cancellationToken = default)
    {
        await _context.DeviceCodes.AddAsync(deviceCode, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public async ValueTask<Result> UpdateAsync(DeviceCodeEntity deviceCode,
        CancellationToken cancellationToken = default)
    {
        _context.DeviceCodes.Update(deviceCode);
        await _context.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }
}