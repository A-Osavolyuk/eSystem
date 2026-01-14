using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.Oidc.Session;

public interface ISessionManager
{
    public ValueTask<SessionEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<SessionEntity?> FindAsync(UserDeviceEntity device, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(UserDeviceEntity device, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(SessionEntity session, CancellationToken cancellationToken = default);
}