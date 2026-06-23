using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authentication.Client;

public interface IClientCommandService
{
    ValueTask<Result> RelateAsync(Guid clientId, Guid sessionId, CancellationToken cancellationToken = default);
}