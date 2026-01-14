using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;
using eSecurity.Core.Security.Authentication.Oidc;
using eSecurity.Core.Security.Authentication.Oidc.Client;
using eSystem.Core.Security.Authentication.Oidc;

namespace eSecurity.Client.Security.Authentication.Oidc;

public interface IConnectService
{
    public ValueTask<HttpResponse<JsonWebKeySet>> GetPublicKeysAsync();
    public ValueTask<HttpResponse<OpenIdConfiguration>> GetOpenidConfigurationAsync();
    public ValueTask<HttpResponse<ClientInfo>> GetClientInfoAsync(string clientId);
    public ValueTask<HttpResponse<AuthorizeResponse>> AuthorizeAsync(AuthorizeRequest request);
    public ValueTask<HttpResponse<TokenResponse>> TokenAsync(TokenRequest request);
    public ValueTask<HttpResponse<LogoutResponse>> LogoutAsync(LogoutRequest request);
}