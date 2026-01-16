using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Oidc.Client;
using eSystem.Core.Security.Authentication.Oidc;
using eSystem.Core.Security.Authentication.Oidc.Token;

namespace eSecurity.Client.Security.Authentication.Oidc;

public interface IConnectService
{
    public ValueTask<ApiResponse<JsonWebKeySet>> GetPublicKeysAsync();
    public ValueTask<ApiResponse<OpenIdConfiguration>> GetOpenidConfigurationAsync();
    public ValueTask<ApiResponse<ClientInfo>> GetClientInfoAsync(string clientId);
    public ValueTask<ApiResponse<AuthorizeResponse>> AuthorizeAsync(AuthorizeRequest request);
    public ValueTask<ApiResponse<TokenResponse>> TokenAsync(TokenRequest request);
    public ValueTask<ApiResponse<LogoutResponse>> LogoutAsync(LogoutRequest request);
}