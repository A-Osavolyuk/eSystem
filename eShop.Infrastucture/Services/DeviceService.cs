using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Enums;
using eShop.Domain.Options;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class DeviceService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IDeviceService
{
    public async ValueTask<Response> TrustAsync(TrustDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/device/trust", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });
}