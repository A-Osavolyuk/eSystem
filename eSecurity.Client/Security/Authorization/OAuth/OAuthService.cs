using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;

namespace eSecurity.Client.Security.Authorization.OAuth;

public class OAuthService(IApiClient apiClient) : IOAuthService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<HttpResponse<LoadOAuthSessionResponse>> LoadSessionAsync(LoadOAuthSessionRequest request)
        => await _apiClient.SendAsync<LoadOAuthSessionResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/OAuth/load"
            }, new HttpOptions()
            {
                Type = DataType.Text,
                Authentication = AuthenticationType.Bearer
            });
}