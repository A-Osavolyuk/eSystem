using eSystem.Auth.Api.Data.Entities;

namespace eSystem.Auth.Api.Security.Authentication.ODIC.Session;

public interface ISessionManager
{
    public ValueTask<SessionEntity?> FindAsync(UserDeviceEntity device, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(UserDeviceEntity device, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(SessionEntity session, CancellationToken cancellationToken = default);
}