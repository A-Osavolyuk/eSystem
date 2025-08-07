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

        var entity = new LoginSessionEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            UserAgent = userAgent,
            IpAddress = ipAddress,
            Provider = provider,
            Device = clientInfo.Device.Family,
            Status = status,
            Type = type,
            Timestamp = DateTimeOffset.UtcNow,
            CreateDate = DateTimeOffset.UtcNow
        };
        
        await context.LoginSessions.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}