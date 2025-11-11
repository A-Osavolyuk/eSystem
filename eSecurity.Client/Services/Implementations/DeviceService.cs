using eSecurity.Client.Common.Http;
using eSecurity.Client.Services.Interfaces;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Services.Implementations;

public class DeviceService(IApiClient apiClient) : IDeviceService
{
    private readonly IApiClient apiClient = apiClient;

    public async ValueTask<HttpResponse> TrustAsync(TrustDeviceRequest request)
        => await apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Device/trust"
            }, new HttpOptions() { Type = DataType.Text });
}