using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.Odic.Session;

public interface ISessionManager
{
    public ValueTask<SessionEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<SessionEntity?> FindAsync(UserDeviceEntity device, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(UserDeviceEntity device, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(SessionEntity session, CancellationToken cancellationToken = default);
}