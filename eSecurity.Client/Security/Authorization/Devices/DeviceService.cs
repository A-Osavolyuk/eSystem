using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;
using eSecurity.Core.Common.Responses;

namespace eSecurity.Client.Security.Authorization.Devices;

public class DeviceService(IApiClient apiClient) : IDeviceService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<HttpResponse<TrustDeviceResponse>> TrustAsync(TrustDeviceRequest request)
        => await _apiClient.SendAsync<TrustDeviceResponse>(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Device/trust"
            }, new HttpOptions()
            {
                ContentType = ContentTypes.Application.Json,
                Authentication = AuthenticationType.None
            });
}