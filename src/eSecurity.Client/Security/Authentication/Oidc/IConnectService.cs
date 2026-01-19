using eSecurity.Core.Common.Requests;
using eSystem.Core.Security.Authentication.Oidc.Token;

namespace eSecurity.Client.Security.Authentication.Oidc;

public interface IConnectService
{
    public ValueTask<ApiResponse> GetPublicKeysAsync();
    public ValueTask<ApiResponse> GetOpenidConfigurationAsync();
    public ValueTask<ApiResponse> GetClientInfoAsync(string clientId);
    public ValueTask<ApiResponse> AuthorizeAsync(AuthorizeRequest request);
    public ValueTask<ApiResponse> TokenAsync(TokenRequest request);
    public ValueTask<ApiResponse> LogoutAsync(LogoutRequest request);
}