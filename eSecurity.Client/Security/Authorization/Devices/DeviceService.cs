using eSecurity.Client.Common.Http;
using eSecurity.Core.Common.Requests;
using eSystem.Core.Common.Http;

namespace eSecurity.Client.Security.Authorization.Devices;

public class DeviceService(IApiClient apiClient) : IDeviceService
{
    private readonly IApiClient _apiClient = apiClient;

    public async ValueTask<Result> TrustAsync(TrustDeviceRequest request)
        => await _apiClient.SendAsync(
            new HttpRequest()
            {
                Method = HttpMethod.Post,
                Data = request,
                Url = "api/v1/Device/trust"
            }, new HttpOptions() { Type = DataType.Text });
}