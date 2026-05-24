using eSecurity.Idp.Data.Entities;
using eSystem.Core.Primitives;

namespace eSecurity.Idp.Security.Authorization.Token.RefreshToken;

public interface IRefreshTokenFlow
{
    public ValueTask<Result> ExecuteAsync(OpaqueTokenEntity token, 
        RefreshTokenFlowContext flowContext, CancellationToken cancellationToken = default);
}