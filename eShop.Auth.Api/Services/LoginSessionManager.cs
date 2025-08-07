using UAParser;

namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILoginSessionManager), ServiceLifetime.Scoped)]
public class LoginSessionManager(AuthDbContext context) : ILoginSessionManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<Result> CreateAsync(UserEntity user, HttpContext httpContext, 
        LoginStatus status, LoginType type, string? provider = null, CancellationToken cancellationToken = default)
    {
        var parser = Parser.GetDefault();
        var userAgent = httpContext.Request.Headers.UserAgent.ToString();
        var ipAddress = httpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()!;
        var clientInfo = parser.Parse(userAgent);

        if (!await context.UserDevices.AnyAsync(x => x.UserId == user.Id, cancellationToken))
        {
            var device = new UserDeviceEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                IsTrusted = true,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Device = clientInfo.Device.ToString(),
                OS = clientInfo.OS.ToString(),
                Browser = clientInfo.UA.ToString(),
                FirstSeen = DateTimeOffset.UtcNow,
                LastSeen = DateTimeOffset.UtcNow,
                CreateDate = DateTimeOffset.UtcNow
            };

            var session = new LoginSessionEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                DeviceId = device.Id,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Type = type,
                Status = status,
                Provider = provider,
                Timestamp = DateTimeOffset.UtcNow,
                CreateDate = DateTimeOffset.UtcNow
            };
            
            await context.UserDevices.AddAsync(device, cancellationToken);
            await context.LoginSessions.AddAsync(session, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        
            return Result.Success();
        }
        
        var existedDevice = await context.UserDevices.FirstOrDefaultAsync(
            x => x.UserId == user.Id 
                 && x.IpAddress == ipAddress 
                 && x.UserAgent == userAgent, cancellationToken);

        if (existedDevice is null)
        {
            var device = new UserDeviceEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                IsTrusted = false,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Device = clientInfo.Device.ToString(),
                OS = clientInfo.OS.ToString(),
                Browser = clientInfo.UA.ToString(),
                FirstSeen = DateTimeOffset.UtcNow,
                LastSeen = DateTimeOffset.UtcNow,
                CreateDate = DateTimeOffset.UtcNow
            };
            
            var session = new LoginSessionEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                DeviceId = device.Id,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Type = type,
                Status = status,
                Provider = provider,
                Timestamp = DateTimeOffset.UtcNow,
                CreateDate = DateTimeOffset.UtcNow
            };
            
            await context.UserDevices.AddAsync(device, cancellationToken);
            await context.LoginSessions.AddAsync(session, cancellationToken);
        }
        else
        {
            if (!existedDevice.IsTrusted)
            {
                return Results.BadRequest("Device is not trusted");
            }
            
            existedDevice.UpdateDate = DateTimeOffset.UtcNow;
            existedDevice.LastSeen = DateTimeOffset.UtcNow;
            
            var session = new LoginSessionEntity()
            {
                Id = Guid.CreateVersion7(),
                UserId = user.Id,
                DeviceId = existedDevice.Id,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Type = type,
                Status = status,
                Provider = provider,
                Timestamp = DateTimeOffset.UtcNow,
                CreateDate = DateTimeOffset.UtcNow
            };
            
            context.UserDevices.Update(existedDevice);
            await context.LoginSessions.AddAsync(session, cancellationToken);
        }

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}