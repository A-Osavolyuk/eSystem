using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;

namespace eSecurity.Client.Security.Authorization.Consent;

public class ConsentService(IApiClient apiClient) : IConsentService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<HttpResponse<CheckConsentResponse>> CheckAsync(CheckConsentRequest request)
        => await _apiClient.SendAsync<CheckConsentResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = "api/v1/Consent/check",
                Data = request
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });

    public async ValueTask<HttpResponse> GrantAsync(GrantConsentRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Url = "api/v1/Consent/grant",
                Data = request
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });
}