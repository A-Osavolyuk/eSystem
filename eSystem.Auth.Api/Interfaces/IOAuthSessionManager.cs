using eSystem.Auth.Api.Data.Entities;

namespace eSystem.Auth.Api.Interfaces;

public interface IOAuthSessionManager
{
    public ValueTask<OAuthSessionEntity?> FindAsync(Guid id, string token, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(OAuthSessionEntity session, CancellationToken cancellationToken = default);
    public ValueTask<Result> UpdateAsync(OAuthSessionEntity session, CancellationToken cancellationToken = default);
}