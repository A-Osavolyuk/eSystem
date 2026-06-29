using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Token;

public interface IOpaqueTokenCommandService
{
    ValueTask<Result> CreateAsync(OpaqueTokenEntity entity, CancellationToken cancellationToken = default);

    ValueTask<Result> RevokeAsync(Guid tokenId, CancellationToken cancellationToken = default);
}