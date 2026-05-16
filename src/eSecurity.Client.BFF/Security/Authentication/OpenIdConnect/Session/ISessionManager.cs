using eSecurity.Client.BFF.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Client.BFF.Security.Authentication.OpenIdConnect.Session;

public interface ISessionManager
{
    ValueTask<SessionEntity?> FindBySidAsync(string sid, CancellationToken cancellationToken = default);
    ValueTask<SessionEntity?> FindByKeyAsync(string key, CancellationToken cancellationToken = default);
    ValueTask<Result> CreateAsync(SessionEntity session, CancellationToken cancellationToken = default);
    ValueTask<Result> UpdateAsync(SessionEntity session, CancellationToken cancellationToken = default);
    ValueTask<Result> DeleteAsync(SessionEntity session, CancellationToken cancellationToken = default);
}