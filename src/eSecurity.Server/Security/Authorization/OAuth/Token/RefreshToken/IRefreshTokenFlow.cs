using eSecurity.Server.Data.Entities;

namespace eSecurity.Server.Security.Authorization.OAuth.Token.RefreshToken;

public interface IRefreshTokenFlow
{
    public ValueTask<Result> ExecuteAsync(OpaqueTokenEntity token, 
        RefreshTokenFlowContext flowContext, CancellationToken cancellationToken = default);
}