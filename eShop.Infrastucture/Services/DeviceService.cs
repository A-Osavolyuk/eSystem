using eShop.Domain.Abstraction.Services;
using eShop.Domain.Common.API;
using eShop.Domain.Common.Http;
using eShop.Domain.Enums;
using eShop.Domain.Requests.API.Auth;

namespace eShop.Infrastructure.Services;

public class DeviceService(
    IConfiguration configuration, 
    IApiClient apiClient) : ApiService(configuration, apiClient), IDeviceService
{
    public async ValueTask<Response> GetAsync(Guid id) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Device/{id}", Method = HttpMethod.Get, },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<Response> TrustAsync(TrustDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Device/trust", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = false, Type = DataType.Text });

    public async ValueTask<Response> BlockAsync(BlockDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Device/block", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> UnblockAsync(UnblockDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Device/unblock", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> VerifyAsync(VerifyDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Device/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });
}