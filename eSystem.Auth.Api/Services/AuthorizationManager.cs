using eSystem.Auth.Api.Data;
using eSystem.Auth.Api.Entities;
using eSystem.Auth.Api.Interfaces;

namespace eSystem.Auth.Api.Services;

[Injectable(typeof(IAuthorizationManager), ServiceLifetime.Scoped)]
public class AuthorizationManager(AuthDbContext context) : IAuthorizationManager
{
    private readonly AuthDbContext context = context;

    public async ValueTask<AuthorizationSessionEntity?> FindAsync(
        UserDeviceEntity device, CancellationToken cancellationToken)
    {
        var entity = await context.AuthorizationSessions.FirstOrDefaultAsync(
            x => x.DeviceId == device.Id, cancellationToken);
        return entity;
    }

    public async ValueTask<Result> CreateAsync(UserDeviceEntity device, CancellationToken cancellationToken)
    {
        var existedAuthorization = await context.AuthorizationSessions.FirstOrDefaultAsync(
            x => x.DeviceId == device.Id, cancellationToken);

        if (existedAuthorization is not null) context.AuthorizationSessions.Remove(existedAuthorization);
        
        var entity = new AuthorizationSessionEntity()
        {
            Id = Guid.CreateVersion7(),
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