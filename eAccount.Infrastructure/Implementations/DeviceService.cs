using eSystem.Core.Common.Network.Gateway;
using eSystem.Core.Requests.Auth;

namespace eAccount.Infrastructure.Implementations;

public class DeviceService(
    GatewayOptions gateway, 
    IApiClient apiClient) : IDeviceService
{
    private readonly GatewayOptions gateway = gateway;
    private readonly IApiClient apiClient = apiClient;
    private const string BasePath = "api/v1/Device";
    
    public async ValueTask<HttpResponse> GetAsync(Guid id) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gateway.Url}/{BasePath}/{id}", Method = HttpMethod.Get, },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> TrustAsync(TrustDeviceRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gateway.Url}/{BasePath}/trust", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> BlockAsync(BlockDeviceRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gateway.Url}/{BasePath}/block", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> UnblockAsync(UnblockDeviceRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gateway.Url}/{BasePath}/unblock", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });

    public async ValueTask<HttpResponse> VerifyAsync(VerifyDeviceRequest request) => await apiClient.SendAsync(
        new HttpRequest { Url = $"{gateway.Url}/{BasePath}/verify", Method = HttpMethod.Post, Data = request },
        new HttpOptions { Type = DataType.Text });
}