using eSecurity.Client.Common.Http;
using eSecurity.Core.Requests;

namespace eSecurity.Client.Security.Authorization.Consent;

public class ConsentService(IApiClient apiClient) : IConsentService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<ApiResponse> CheckAsync(CheckConsentRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Url = "/api/v1/Consent/check",
                Data = request
            });

    public async ValueTask<ApiResponse> GrantAsync(GrantConsentRequest request)
        => await _apiClient.SendAsync(
            new ApiRequest
            {
                Method = HttpMethods.Post,
                Url = "/api/v1/Consent/grant",
                Data = request
            });
}