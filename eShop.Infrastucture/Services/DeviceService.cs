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

    public async ValueTask<Response> BlockAsync(BlockDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/device/block", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> UnblockAsync(UnblockDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/device/unblock", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> VerifyAsync(VerifyDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/device/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> ConfirmVerifyAsync(ConfirmVerifyDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/device/confirm-verification", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> ConfirmBlockAsync(ConfirmBlockDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/device/confirm-block", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });

    public async ValueTask<Response> ConfirmUnblockAsync(ConfirmUnblockDeviceRequest request) => await ApiClient.SendAsync(
        new HttpRequest { Url = $"{Gateway}/api/v1/Security/device/confirm-unblock", Method = HttpMethod.Post, Data = request },
        new HttpOptions { WithBearer = true, Type = DataType.Text });
}