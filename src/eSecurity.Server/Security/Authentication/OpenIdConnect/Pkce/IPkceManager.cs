using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authentication.OpenIdConnect.Pkce;

public interface IPkceManager
{
    public ValueTask<bool> IsVerified(Guid clientId, Guid sessionId, CancellationToken cancellationToken = default);
    public ValueTask<Result> CreateAsync(ClientPkceStateEntity entity, CancellationToken cancellationToken = default);
}