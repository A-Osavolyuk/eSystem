namespace eShop.Auth.Api.Services;

[Injectable(typeof(IAuthorizationManager), ServiceLifetime.Scoped)]
public class AuthorizationManager(AuthDbContext context) : IAuthorizationManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<AuthorizationSessionEntity?> FindAsync(UserEntity user, 
        UserDeviceEntity device, CancellationToken cancellationToken)
    {
        var entity = await context.AuthorizationSessions.FirstOrDefaultAsync(
            x => x.UserId == user.Id && x.DeviceId == device.Id, cancellationToken);
        return entity;
    }

    public async ValueTask<Result> CreateAsync(UserEntity user, 
        UserDeviceEntity device, CancellationToken cancellationToken)
    {
        var entity = new AuthorizationSessionEntity()
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            DeviceId = device.Id,
            CreateDate = DateTimeOffset.UtcNow,
        };
        
        await context.AuthorizationSessions.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    public async ValueTask<Result> RemoveAsync(AuthorizationSessionEntity session, CancellationToken cancellationToken)
    {
        context.AuthorizationSessions.Remove(session);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}