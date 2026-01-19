using eSystem.Core.Http;
using eSystem.Core.Security.Authentication.Oidc.Token;

namespace eCinema.Server.Security.Authentication.Oidc;

public interface IConnectService
{
    public ValueTask<ApiResponse> TokenAsync(TokenRequest request, CancellationToken cancellationToken = default);
    public ValueTask<ApiResponse> GetUserInfoAsync(CancellationToken cancellationToken = default);
}