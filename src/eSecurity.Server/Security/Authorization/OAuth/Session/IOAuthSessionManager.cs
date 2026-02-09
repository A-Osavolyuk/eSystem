using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.OAuth.Session;

public interface IOAuthSessionManager
{
    public ValueTask<OAuthSessionEntity?> FindByIdAsync(Guid id, CancellationToken cancellationToken);
    public ValueTask<Result> CreateAsync(OAuthSessionEntity session, CancellationToken cancellationToken);
    public ValueTask<Result> UpdateAsync(OAuthSessionEntity session, CancellationToken cancellationToken);
}