using eSecurity.Idp.Data.Entities;

namespace eSecurity.Idp.Security.Authentication.Session;

public interface ISessionQueryService
{
    ValueTask<SessionEntity?> GetByIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

    ValueTask<bool> HasClientAsync(Guid sessionId, Guid clientId, CancellationToken cancellationToken = default);
}