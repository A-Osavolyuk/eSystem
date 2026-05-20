using eSecurity.Client.Common.Http;

namespace eSecurity.Client.Security.Authentication.OpenIdConnect;

public interface IConnectService
{
    public ValueTask<ApiResponse> GetClientInfoAsync(string clientId);
    public ValueTask<ApiResponse> UserInfoAsync();
}