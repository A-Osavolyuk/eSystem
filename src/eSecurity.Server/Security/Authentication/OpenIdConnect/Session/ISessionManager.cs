using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Session;

public interface ISessionManager
{
    public ValueTask<SessionEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken = default);
    public ValueTask<SessionEntity?> FindAsync(UserEntity user, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(SessionEntity session, CancellationToken cancellationToken = default);
    public ValueTask<Result> RemoveAsync(SessionEntity session, CancellationToken cancellationToken = default);
}