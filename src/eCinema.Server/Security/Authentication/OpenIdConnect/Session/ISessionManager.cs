using eCinema.Server.Data.Entities;
using eSystem.Core.Primitives;

namespace eCinema.Server.Security.Authentication.OpenIdConnect.Session;

public interface ISessionManager
{
    public ValueTask<SessionEntity?> FindBySidAsync(string sid, CancellationToken cancellationToken = default);
    public ValueTask<SessionEntity?> FindByKeyAsync(string key, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(SessionEntity session, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(SessionEntity session, CancellationToken cancellationToken = default);
    public ValueTask<Result> DeleteAsync(SessionEntity session, CancellationToken cancellationToken = default);
}