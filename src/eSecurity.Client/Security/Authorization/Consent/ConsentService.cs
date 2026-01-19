using eSecurity.Core.Common.Requests;

namespace eSecurity.Client.Security.Authorization.Consent;

public class ConsentService(IApiClient apiClient) : IConsentService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> CheckAsync(CheckConsentRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Url = "/api/v1/Consent/check",
                Data = request
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<ApiResponse> GrantAsync(GrantConsentRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest()
            {
                Method = HttpMethod.Post,
                Url = "/api/v1/Consent/grant",
                Data = request
            }, new ApiOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });
}