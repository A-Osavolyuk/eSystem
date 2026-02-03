using eSecurity.Server.Data.Entities;
using eSystem.Core.Http.Results;

namespace eSecurity.Server.Security.Authorization.Token.RefreshToken;

public interface IRefreshTokenFlow
{
    public ValueTask<Result> ExecuteAsync(OpaqueTokenEntity token, 
        RefreshTokenFlowContext flowContext, CancellationToken cancellationToken = default);
}