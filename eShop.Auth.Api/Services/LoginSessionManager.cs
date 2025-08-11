namespace eShop.Auth.Api.Services;

[Injectable(typeof(ILoginSessionManager), ServiceLifetime.Scoped)]
public class LoginSessionManager(AuthDbContext context) : ILoginSessionManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<Result> CreateAsync(UserEntity user, HttpContext httpContext, 
        LoginStatus status, LoginType type, string? provider = null, CancellationToken cancellationToken = default)
    {
        var userAgent = RequestUtils.GetUserAgent(httpContext);
        var ipAddress = RequestUtils.GetIpV4(httpContext);
        
        var device = await context.UserDevices.FirstOrDefaultAsync(
            x => x.UserId == user.Id 
                 && x.IpAddress == ipAddress 
                 && x.UserAgent == userAgent, cancellationToken);

        if (device is null)
        {
            return Results.InternalServerError("Failed to create login session. Device not found.");
        }

        if (!device.IsTrusted)
        {
            return Results.BadRequest("Device is not trusted");
        }
            
        device.UpdateDate = DateTimeOffset.UtcNow;
        device.LastSeen = DateTimeOffset.UtcNow;
            
        var session = new LoginSessionEntity()
        {
            Id = Guid.CreateVersion7(),
            DeviceId = device.Id,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Type = type,
            Status = status,
            Provider = provider,
            Timestamp = DateTimeOffset.UtcNow,
            CreateDate = DateTimeOffset.UtcNow
        };
            
        context.UserDevices.Update(device);
        await context.LoginSessions.AddAsync(session, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}