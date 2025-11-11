using eSecurity.Client.Common.Http;
using eSecurity.Client.Services.Interfaces;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Services.Implementations;

public class OAuthService(IApiClient apiClient) : IOAuthService
{
    private readonly IApiClient apiClient = apiClient;

    public async ValueTask<HttpResponse> LoadSessionAsync(LoadOAuthSessionRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/OAuth/load"
            }, new HttpOptions() { Type = DataType.Text });
}